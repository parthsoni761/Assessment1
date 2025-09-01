using Microsoft.AspNetCore.Mvc;
using ListWatch.Services;
using ListWatch.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ListWatch.Controllers
{
    [ApiController]
    // The base route is good
    [Route("api/WatchListItems")]
    [Authorize]
    public class WatchListController : ControllerBase
    {
        private readonly IWatchListService _service;
        private readonly IMapper _mapper;
        public WatchListController(IWatchListService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // UPDATED: GET api/WatchListItems/user/{userId}?status=Completed&search=...
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<WatchListItemDto>>> GetAllByUserIdAsync(
            int userId,
            [FromQuery] string? status,
            [FromQuery] string? genre,
            [FromQuery] string? type,
            [FromQuery] bool? isFavorite,
            [FromQuery] string? search,
            [FromQuery] string? sortBy)
        {
            var items = await _service.GetAllByUserIdAsync(userId, status, genre, type, isFavorite, search, sortBy);
            return Ok(items);
        }

        // This gets a single item by its own ID, which is standard REST practice
        [HttpGet("{id}")]
        public async Task<ActionResult<WatchListItemDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
        public async Task<ActionResult<WatchListItemDto>> CreateAsync(CreateWatchListItemDto dto)
        {
            var createdItem = await _service.CreateAsync(dto);
            return Ok(createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateWatchListItemDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}