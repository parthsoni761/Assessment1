using ListWatch.Controllers;
using ListWatch.DTOs;
using ListWatch.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ListWatch.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _dashboardServiceMock;
        private readonly Mock<IExternalApiService> _externalApiServiceMock;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _dashboardServiceMock = new Mock<IDashboardService>();
            _externalApiServiceMock = new Mock<IExternalApiService>();
            _controller = new DashboardController(_dashboardServiceMock.Object, _externalApiServiceMock.Object);
        }

        // --- TESTS for GET summary/{userId} ---
        [Fact]
        public async Task GetSummary_ReturnsOk_WithDashboardDto()
        {
            // Arrange
            var userId = 1;
            var dashboardDto = new DashboardDto { TotalItems = 5 };

            _dashboardServiceMock
                .Setup(s => s.GetSummaryAsync(
                    userId,
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(dashboardDto);

            // Act
            var result = await _controller.GetSummary(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<DashboardDto>(okResult.Value);
            Assert.Equal(5, returnedDto.TotalItems);
        }

        // --- TESTS for GET popular ---
        [Fact]
        public async Task GetPopular_ReturnsOk_WithResults()
        {
            // Arrange
            var popularItems = new List<ExternalApiResultDto>
            {
                new ExternalApiResultDto { Title = "Item1" },
                new ExternalApiResultDto { Title = "Item2" }
            };

            _externalApiServiceMock
                .Setup(s => s.GetPopularAsync())
                .ReturnsAsync(popularItems);

            // Act
            var result = await _controller.GetPopular();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert.IsAssignableFrom<IEnumerable<ExternalApiResultDto>>(okResult.Value);
            Assert.Collection(returnedItems,
                item => Assert.Equal("Item1", item.Title),
                item => Assert.Equal("Item2", item.Title));
        }

        // --- TESTS for GET search ---
        [Fact]
        public async Task Search_WithValidQuery_ReturnsOk_WithResults()
        {
            // Arrange
            string query = "Batman";
            var searchResults = new List<ExternalApiResultDto>
            {
                new ExternalApiResultDto { Title = "Batman Begins" }
            };

            _externalApiServiceMock
                .Setup(s => s.SearchAsync(query))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert.IsAssignableFrom<IEnumerable<ExternalApiResultDto>>(okResult.Value);
            Assert.Single(returnedItems);
        }

        [Fact]
        public async Task Search_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Search("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search query cannot be empty.", badRequestResult.Value);
        }
    }
}
