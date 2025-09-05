using ListWatch.DTOs;
using System.Text.Json;
using System.Threading.Tasks;

namespace ListWatch.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // Helper classes to deserialize OMDb's unique JSON structures
        private class OmdbSearchItem { public string Title { get; set; } = ""; public string Year { get; set; } = ""; public string Type { get; set; } = ""; }
        private class OmdbSearchResponse { public List<OmdbSearchItem> Search { get; set; } = new(); public string Response { get; set; } = "False"; }
        private class OmdbItemResponse { public string Title { get; set; } = ""; public string Year { get; set; } = ""; public string Type { get; set; } = ""; public string Genre { get; set; } = ""; public string Response { get; set; } = "False"; }

        public ExternalApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDb API key is not configured.");
        }

        public async Task<IEnumerable<ExternalApiResultDto>> GetPopularAsync()
        {
            // This is our curated list of high-quality, popular IMDb IDs.
            // This is the "secret sauce" to creating a great "popular" list from OMDb.
            // You can change these IDs anytime to refresh the content.
            var popularIds = new List<string>
            {
                "tt0110912", // Pulp Fiction
                "tt0111161", // The Shawshank Redemption
                "tt0468569", // The Dark Knight
                "tt0903747", // Breaking Bad
                "tt0944947", // Game of Thrones
                "tt0137523", // Fight Club
                "tt1375666", // Inception
                "tt7286456", // Joker
                "tt4154796", // Avengers: Endgame
                "tt2543164"  // Arrival
            };

            // We create a list of tasks, where each task is an API call to fetch one item.
            var tasks = popularIds.Select(id => FetchItemByIdAsync(id));

            // Task.WhenAll runs all the API calls in parallel for maximum speed.
            var results = await Task.WhenAll(tasks);

            // We filter out any nulls that might have occurred from a failed API call and return the list.
            return results.Where(r => r != null).ToList()!;
        }

        public async Task<IEnumerable<ExternalApiResultDto>> SearchAsync(string query)
        {
            var response = await _httpClient.GetAsync($"https://www.omdbapi.com/?apikey={_apiKey}&s={Uri.EscapeDataString(query)}");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<ExternalApiResultDto>();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var omdbResponse = JsonSerializer.Deserialize<OmdbSearchResponse>(content, options);

            if (omdbResponse == null || omdbResponse.Response != "True")
            {
                return Enumerable.Empty<ExternalApiResultDto>();
            }

            // FIX #1: Explicitly filter for only movies and series to avoid other types like "game".
            return omdbResponse.Search
                .Where(result => result.Type == "movie" || result.Type == "series")
                .Select(result => new ExternalApiResultDto
                {
                    Title = result.Title,
                    ItemType = result.Type == "series" ? "TV Show" : "Movie",
                    // FIX #2: Add a safety check before parsing the year to prevent crashes.
                    ReleaseYear = !string.IsNullOrEmpty(result.Year) && result.Year.Length >= 4 && int.TryParse(result.Year.AsSpan(0, 4), out var year)
                                  ? year
                                  : null,
                    Genre = " "
                });
        }

        // This private helper method fetches the full details for a single IMDb ID.
        private async Task<ExternalApiResultDto?> FetchItemByIdAsync(string imdbId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://www.omdbapi.com/?apikey={_apiKey}&i={imdbId}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var itemResponse = JsonSerializer.Deserialize<OmdbItemResponse>(content, options);

                if (itemResponse == null || itemResponse.Response != "True") return null;

                return new ExternalApiResultDto
                {
                    Title = itemResponse.Title,
                    ItemType = itemResponse.Type == "series" ? "TV Show" : "Movie",
                    ReleaseYear = int.TryParse(itemResponse.Year.AsSpan(0, 4), out var year) ? year : null,
                    // We can get the primary genre from the detailed view
                    Genre = itemResponse.Genre.Split(',').FirstOrDefault()?.Trim() ?? ""
                };
            }
            catch (Exception ex)
            {
                // Log the exception for debugging, but don't crash the whole process.
                Console.WriteLine($"Error fetching item {imdbId}: {ex.Message}");
                return null;
            }
        }
    }
}