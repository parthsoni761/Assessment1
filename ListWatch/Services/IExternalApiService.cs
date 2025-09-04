using ListWatch.DTOs;

namespace ListWatch.Services
{
    public interface IExternalApiService
    {
        Task<IEnumerable<ExternalApiResultDto>> GetPopularAsync();
        Task<IEnumerable<ExternalApiResultDto>> SearchAsync(string query);
    }
}