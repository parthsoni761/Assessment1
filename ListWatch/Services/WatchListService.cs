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
        private static bool _titleSortAscending = true;

        public WatchListService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }



        public async Task<WatchListItemDto?> GetByIdAsync(int id)
        {
            var item = await _db.WatchListItems
                .Include(w => w.WatchProgress)
                .FirstOrDefaultAsync(w => w.Id == id); // Correctly finds by item Id

            return item == null ? null : _mapper.Map<WatchListItemDto>(item);
        }


        // This is the new method that gets ALL items for a user with filtering.
        public async Task<IEnumerable<WatchListItemDto>> GetAllByUserIdAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortBy = null)
        {
            var query = _db.WatchListItems.AsQueryable()
                .Include(w => w.WatchProgress)
                .Where(w => w.UserId == userId);

            // Filtering
            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(i => i.Genre == genre);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(i => i.ItemType == type);

            if (isFavorite.HasValue)
                query = query.Where(i => i.IsFavorite == isFavorite.Value);

            // Searching
            if (!string.IsNullOrEmpty(search))
                query = query.Where(i => i.Title.Contains(search));

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "releaseyear" => query.OrderBy(i => i.ReleaseYear),
                "rating" => query.OrderByDescending(i => i.Rating),
                "title_asc" => query.OrderBy(i => i.Title),
                "title_desc" => query.OrderByDescending(i => i.Title),
                
                _ => query.OrderBy(i => i.Id)

            };

            if (sortBy?.ToLower() == "title")
            {
                _titleSortAscending = !_titleSortAscending;
            }

            var items = await query.ToListAsync();
            return _mapper.Map<IEnumerable<WatchListItemDto>>(items);
        }

        public async Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto)
        {
            // Map all properties from the DTO to the main entity
            var item = _mapper.Map<WatchListItems>(dto);

            // If the item is a TV Show and has episode counts, create a WatchProgress entity
            if (dto.ItemType?.Equals("TV Show", StringComparison.OrdinalIgnoreCase) == true)
            {
                item.WatchProgress = new WatchProgress
                {
                    // Use the DTO values, defaulting to 0 if they are null
                    CompletedEpisodes = dto.CompletedEpisodes ?? 0,
                    TotalEpisodes = dto.TotalEpisodes ?? 0
                };
            }

            _db.WatchListItems.Add(item);
            await _db.SaveChangesAsync();

            // Map the final result back to a DTO to return to the client
            return _mapper.Map<WatchListItemDto>(item);
        }


        public async Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto)
        {
            // Eagerly load the related WatchProgress data
            var item = await _db.WatchListItems
                .Include(w => w.WatchProgress)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (item == null)
            {
                return false;
            }

            // Use AutoMapper to update all the flat properties (Title, Status, etc.)
            _mapper.Map(dto, item);

            // Now, handle the WatchProgress logic
            bool isTvShow = dto.ItemType?.Equals("TV Show", StringComparison.OrdinalIgnoreCase) == true;

            if (isTvShow)
            {
                // If it's a TV show, ensure WatchProgress exists
                if (item.WatchProgress == null)
                {
                    item.WatchProgress = new WatchProgress();
                }
                // Update the episode counts
                item.WatchProgress.CompletedEpisodes = dto.CompletedEpisodes ?? 0;
                item.WatchProgress.TotalEpisodes = dto.TotalEpisodes ?? 0;
            }
            else
            {
                // If it's NOT a TV show and has progress, remove it
                if (item.WatchProgress != null)
                {
                    _db.WatchProgress.Remove(item.WatchProgress);
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }








        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.WatchListItems.FindAsync(id);
            if (item == null) return false;



            if(item.WatchProgress != null)
            {
                _db.WatchProgress.Remove(item.WatchProgress);
            }
            _db.WatchListItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
