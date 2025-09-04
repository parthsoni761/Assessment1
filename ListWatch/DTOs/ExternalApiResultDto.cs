namespace ListWatch.DTOs
{
    public class ExternalApiResultDto
    {
        public string Title { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty; // "Movie" or "TV Show"
        public int? ReleaseYear { get; set; }
        public string Genre { get; set; }= string.Empty;
        public int Rating { get; set; } // 1 to 5 stars
        
    }
}