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
    public class WatchListControllerTests
    {
        private readonly Mock<IWatchListService> _serviceMock;
        private readonly WatchListController _controller;

        public WatchListControllerTests()
        {
            _serviceMock = new Mock<IWatchListService>();
            _controller = new WatchListController(_serviceMock.Object);
        }

        // --- GET user/{userId} ---
        [Fact]
        public async Task GetAllByUserIdAsync_ReturnsOk_WithItems()
        {
            var userId = 1;
            var items = new List<WatchListItemDto> { new WatchListItemDto { Id = 1, Title = "Test" } };

            _serviceMock.Setup(s => s.GetAllByUserIdAsync(userId, null, null, null, null, null, null, null))
                        .ReturnsAsync(items);

            var result = await _controller.GetAllByUserIdAsync(userId, null, null, null, null, null, null, null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<WatchListItemDto>>(ok.Value);
            Assert.Single(returned);
        }

        // --- GET {id} ---
        [Fact]
        public async Task GetByIdAsync_ItemExists_ReturnsOk()
        {
            var item = new WatchListItemDto { Id = 1, Title = "Movie" };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(item);

            var result = await _controller.GetByIdAsync(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<WatchListItemDto>(ok.Value);
            Assert.Equal("Movie", returned.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ItemNotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((WatchListItemDto?)null);

            var result = await _controller.GetByIdAsync(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // --- PATCH toggleFavorite ---
        [Fact]
        public async Task ToggleFavorite_ItemExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.ToggleFavoriteAsync(1)).ReturnsAsync(true);

            var result = await _controller.ToggleFavorite(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ToggleFavorite_ItemNotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.ToggleFavoriteAsync(1)).ReturnsAsync(false);

            var result = await _controller.ToggleFavorite(1);

            Assert.IsType<NotFoundResult>(result);
        }

        // --- POST ---
        [Fact]
        public async Task CreateAsync_ReturnsOk_WithCreatedItem()
        {
            var dto = new CreateWatchListItemDto { Title = "New Movie" };
            var created = new WatchListItemDto { Id = 1, Title = "New Movie" };

            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreateAsync(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<WatchListItemDto>(ok.Value);
            Assert.Equal("New Movie", returned.Title);
        }

        // --- PUT ---
        [Fact]
        public async Task UpdateAsync_ItemExists_ReturnsNoContent()
        {
            var dto = new UpdateWatchListItemDto { Title = "Updated" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateAsync(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ItemNotFound_ReturnsNotFound()
        {
            var dto = new UpdateWatchListItemDto { Title = "Updated" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(false);

            var result = await _controller.UpdateAsync(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        // --- DELETE ---
        [Fact]
        public async Task DeleteAsync_ItemExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteAsync(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ItemNotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteAsync(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
