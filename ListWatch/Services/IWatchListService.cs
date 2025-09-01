using ListWatch.DTOs;

namespace ListWatch.Services
{
    public interface IWatchListService
    {
        // Renamed and updated to support filtering
        Task<IEnumerable<WatchListItemDto>> GetAllByUserIdAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortBy = null);

        Task<WatchListItemDto?> GetByIdAsync(int id); // Kept the original GetById for single item fetching
        Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto);
        Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}