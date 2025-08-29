using ListWatch.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
namespace ListWatch.Services
{
    public interface IWatchListService
    {
        Task<IEnumerable<WatchListItemDto>> GetAllAsync();
        Task<List<WatchListItemDto?>> GetByIdAsync(int id);
        Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto);
        Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
