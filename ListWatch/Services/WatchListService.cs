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




        public async Task<IEnumerable<WatchListItemDto>> GetAllAsync()
        {
            var items = await _db.WatchListItems
                .Include(w =>w.WatchProgress)
                .ToListAsync();
            return _mapper.Map<IEnumerable<WatchListItemDto>>(items);
        }




        public async Task<List<WatchListItemDto?>?> GetByIdAsync(int id)
        {
            var items = await _db.WatchListItems
                .Include(w => w.WatchProgress)              // Include related progress data
                .Where(w => w.UserId == id)             // Filter by UserId
                .ToListAsync();                             // Get all matching entries

            return items == null ? null : _mapper.Map<List<WatchListItemDto>>(items); // Map to DTO list
        }





        public async Task<WatchListItemDto> CreateAsync(CreateWatchListItemDto dto)
        {
            var item = _mapper.Map<WatchListItems>(dto);
            _db.WatchListItems.Add(item);

            if(dto.ItemType?.ToLower() == "show")
            {
                item.WatchProgress = new WatchProgress
                {
                    CompletedEpisodes = dto.CompletedEpisodes ?? 0,
                    TotalEpisodes = dto.TotalEpisodes ?? 0
                };
            }
            _db.WatchListItems.Add(item);
            await _db.SaveChangesAsync();


            var resultDto = _mapper.Map<WatchListItemDto>(item);
            if (item.WatchProgress!=null)
            {
                resultDto.CompletedEpisodes = item.WatchProgress.CompletedEpisodes;
                resultDto.TotalEpisodes = item.WatchProgress.TotalEpisodes;
            }
            return resultDto;
        }









        public async Task<bool> UpdateAsync(int id, UpdateWatchListItemDto dto)
        {
            // Include WatchProgress to track it properly
            var item = await _db.WatchListItems
                .Include(w => w.WatchProgress)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (item == null) return false;

            // Map the basic properties
            _mapper.Map(dto, item);

            if (item.ItemType?.ToLower() == "show")
            {
                // If no existing WatchProgress, create one
                if (item.WatchProgress == null)
                {
                    item.WatchProgress = new WatchProgress
                    {
                        CompletedEpisodes = dto.CompletedEpisodes ?? 0,
                        TotalEpisodes = dto.TotalEpisodes ?? 0
                    };
                }
                else
                {
                    // Update existing WatchProgress
                    item.WatchProgress.CompletedEpisodes = dto.CompletedEpisodes ?? item.WatchProgress.CompletedEpisodes;
                    item.WatchProgress.TotalEpisodes = dto.TotalEpisodes ?? item.WatchProgress.TotalEpisodes;
                }
            }
            else
            {
                // Remove WatchProgress if item is not a show
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
