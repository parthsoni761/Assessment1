using ListWatch.DTOs;

namespace ListWatch.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetSummaryAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortBy = null);
    }
}
