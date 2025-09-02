using Microsoft.AspNetCore.Mvc;
using ListWatch.Services;
using ListWatch.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ListWatch.Controllers
{
    [ApiController]
    [Route("api/WatchListItems")]
    [Authorize]
    public class WatchListController : ControllerBase
    {
        private readonly IWatchListService _service;
        public WatchListController(IWatchListService service)
        {
            _service = service;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<WatchListItemDto>>> GetAllByUserIdAsync(
            int userId,
            [FromQuery] string? status,
            [FromQuery] string? genre,
            [FromQuery] string? type,
            [FromQuery] bool? isFavorite,
            [FromQuery] string? search,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortDirection)
        {
            var items = await _service.GetAllByUserIdAsync(userId, status, genre, type, isFavorite, search, sortColumn, sortDirection);
            return Ok(items);
        }

        [HttpGet("{id}", Name = "GetWatchlistItemById")]
        public async Task<ActionResult<WatchListItemDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // --- NEW ENDPOINT for toggling favorite ---
        [HttpPatch("{id}/toggleFavorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var success = await _service.ToggleFavoriteAsync(id);
            if (!success) return NotFound();
            return NoContent(); // Success, no content to return
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