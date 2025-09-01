using AutoMapper;
using ListWatch.DTOs;
using ListWatch.Data;
using ListWatch.Models;
using Microsoft.EntityFrameworkCore;

namespace ListWatch.Services
{
    public class WatchListService : IWatchListService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        public WatchListService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WatchListItemDto>> GetAllByUserIdAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortColumn = null,
            string? sortDirection = null)
        {
            var query = _db.WatchListItems.AsQueryable()
                .Include(w => w.WatchProgress)
                .Where(w => w.UserId == userId);

            // Filtering
            if (!string.IsNullOrEmpty(status)) query = query.Where(i => i.Status == status);
            if (!string.IsNullOrEmpty(genre)) query = query.Where(i => i.Genre == genre);
            if (!string.IsNullOrEmpty(type)) query = query.Where(i => i.ItemType == type);
            if (isFavorite.HasValue) query = query.Where(i => i.IsFavorite == isFavorite.Value);
            if (!string.IsNullOrEmpty(search)) query = query.Where(i => i.Title.Contains(search));

            // Server-side sorting logic
            if (!string.IsNullOrEmpty(sortColumn))
            {
                bool isAscending = sortDirection?.ToLower() == "asc";

                query = sortColumn.ToLower() switch
                {
                    "title" => isAscending ? query.OrderBy(i => i.Title) : query.OrderByDescending(i => i.Title),
                    "releaseyear" => isAscending ? query.OrderBy(i => i.ReleaseYear) : query.OrderByDescending(i => i.ReleaseYear),
                    "rating" => isAscending ? query.OrderBy(i => i.Rating) : query.OrderByDescending(i => i.Rating),
                    _ => query.OrderBy(i => i.Id)
                };
            }
            else
            {
                query = query.OrderByDescending(i => i.Rating); // Default sort
            }

            var items = await query.ToListAsync();
            return _mapper.Map<IEnumerable<WatchListItemDto>>(items);
        }

        public async Task<WatchListItemDto?> GetByIdAsync(int id)
        {
            var item = await _db.WatchListItems.Include(w => w.WatchProgress).FirstOrDefaultAsync(w => w.Id == id);
            return item == null ? null : _mapper.Map<WatchListItemDto>(item);
        }

        // --- NEW METHOD for toggling IsFavorite ---
        public async Task<bool> ToggleFavoriteAsync(int id)
        {
            var item = await _db.WatchListItems.FindAsync(id);
            if (item == null) return false;

            item.IsFavorite = !item.IsFavorite; // Flip the boolean
            await _db.SaveChangesAsync();
            return true;
        }

        // Create, Update, and Delete methods remain the same as before
        public async Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto)
        {
            var item = _mapper.Map<WatchListItems>(dto);
            if (dto.ItemType?.Equals("TV Show", StringComparison.OrdinalIgnoreCase) == true)
            {
                item.WatchProgress = new WatchProgress { CompletedEpisodes = dto.CompletedEpisodes ?? 0, TotalEpisodes = dto.TotalEpisodes ?? 0 };
            }
            _db.WatchListItems.Add(item);
            await _db.SaveChangesAsync();
            return _mapper.Map<WatchListItemDto>(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto)
        {
            var item = await _db.WatchListItems.Include(w => w.WatchProgress).FirstOrDefaultAsync(w => w.Id == id);
            if (item == null) return false;
            _mapper.Map(dto, item);
            bool isTvShow = dto.ItemType?.Equals("TV Show", StringComparison.OrdinalIgnoreCase) == true;
            if (isTvShow)
            {
                if (item.WatchProgress == null) item.WatchProgress = new WatchProgress();
                item.WatchProgress.CompletedEpisodes = dto.CompletedEpisodes ?? 0;
                item.WatchProgress.TotalEpisodes = dto.TotalEpisodes ?? 0;
            }
            else if (item.WatchProgress != null)
            {
                _db.WatchProgress.Remove(item.WatchProgress);
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.WatchListItems.Include(i => i.WatchProgress).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return false;
            if (item.WatchProgress != null) _db.WatchProgress.Remove(item.WatchProgress);
            _db.WatchListItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}