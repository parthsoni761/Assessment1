using ListWatch.Data;
using ListWatch.DTOs;
using ListWatch.Models;
using Microsoft.EntityFrameworkCore;

namespace ListWatch.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;
        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetSummaryAsync(
            int userId,
            string? status = null,
            string? genre = null,
            string? type = null,
            bool? isFavorite = null,
            string? search = null,
            string? sortBy = null)
        {
            var query = _db.WatchListItems.AsQueryable()
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
                "title" => query.OrderBy(i => i.Title),
                _ => query
            };

            var items = await query.ToListAsync();






            // Summary calculations
            var totalItems = items.Count;
            var completed = items.Count(i => i.Status.Equals("completed", StringComparison.OrdinalIgnoreCase));
            var pending = items.Count(i => !i.Status.Equals("completed", StringComparison.OrdinalIgnoreCase));
            var favorites = items.Count(i => i.IsFavorite);

            var favoriteGenre = items
                .GroupBy(i => i.Genre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? string.Empty;

            var topRated = await _db.WatchListItems
        .Include(i => i.WatchProgress)
        .OrderByDescending(i => i.Rating)
        .Take(5)
        .Select(i => new WatchListItemDto
        {
            Id = i.Id,
            UserId = i.UserId,
            Title = i.Title,
            ItemType = i.ItemType,
            Genre = i.Genre,
            ReleaseYear = i.ReleaseYear,
            Status = i.Status,
            Rating = i.Rating ?? 0,
            CompletedEpisodes = i.WatchProgress != null ? i.WatchProgress.CompletedEpisodes : 0,
            TotalEpisodes = i.WatchProgress != null ? i.WatchProgress.TotalEpisodes : 0,
            IsFavorite = i.IsFavorite
        })
        .ToListAsync();

            return new DashboardDto
            {
                TotalItems = totalItems,
                CompletedItems = completed,
                PendingItems = pending,
                FavoriteItems = favorites,
                FavoriteGenre = favoriteGenre,
                TopRatedItems = topRated
            };
        }
    }
}
