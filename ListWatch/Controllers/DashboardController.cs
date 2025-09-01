using ListWatch.DTOs;
using ListWatch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/dashboard/summary/{userId}?status=completed&type=movie&isFavorite=true&sortBy=rating
        [HttpGet("summary/{userId}")]
        public async Task<ActionResult<DashboardDto>> GetSummary(
            int userId,
            [FromQuery] string? status,
            [FromQuery] string? genre,
            [FromQuery] string? type,
            [FromQuery] bool? isFavorite,
            [FromQuery] string? search,
            [FromQuery] string? sortBy)
        {
            var result = await _dashboardService.GetSummaryAsync(userId, status, genre, type, isFavorite, search, sortBy);
            return Ok(result);
        }
    }
}
