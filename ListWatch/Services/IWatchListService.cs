using ListWatch.DTOs;

namespace ListWatch.Services
{
    public interface IWatchListService
    {
        Task<IEnumerable<WatchListItemDto>> GetAllByUserIdAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortColumn = null,
            string? sortDirection = null);

        Task<WatchListItemDto?> GetByIdAsync(int id);
        Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto);
        Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleFavoriteAsync(int id); // New method for favorite toggle
    }
}