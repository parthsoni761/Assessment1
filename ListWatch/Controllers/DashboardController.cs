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
        private readonly IExternalApiService _externalApiService; 

        public DashboardController(IDashboardService dashboardService, IExternalApiService externalApiService)
        {
            _dashboardService = dashboardService;
            _externalApiService = externalApiService; 
        }

        
        [HttpGet("summary/{userId}")]
        public async Task<ActionResult<DashboardDto>> GetSummary(int userId)
        {
            var result = await _dashboardService.GetSummaryAsync(userId);
            return Ok(result);
        }

        // --- NEW ENDPOINT for getting popular items ---
        // GET: api/Dashboard/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular()
        {
            var results = await _externalApiService.GetPopularAsync();
            return Ok(results);
        }

        // --- NEW ENDPOINT for searching items ---
        // GET: api/Dashboard/search?query=...
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }
            var results = await _externalApiService.SearchAsync(query);
            return Ok(results);
        }
    }
}