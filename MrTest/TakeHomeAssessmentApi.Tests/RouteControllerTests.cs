using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TakeHomeAssessmentApi.Controllers;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class RouteControllerTests
    {
        private static RouteDto GetRouteDto() => new()
        {
            Id = 1,
            VehicleId = "V1",
            Path = "A->B",
            Distance = 10.5,
            CalculatedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task AddRoute_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IRouteService>();
            var dto = GetRouteDto();
            mockService.Setup(s => s.AddRouteAsync(dto)).Returns(Task.CompletedTask);

            var controller = new RouteController(mockService.Object);

            var result = await controller.AddRoute(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Ruta registrada.", okResult.Value);
        }

        [Fact]
        public async Task AddRoute_ThrowsException_WhenServiceFails()
        {
            var mockService = new Mock<IRouteService>();
            var dto = GetRouteDto();
            mockService.Setup(s => s.AddRouteAsync(dto)).ThrowsAsync(new Exception("Error"));

            var controller = new RouteController(mockService.Object);

            await Assert.ThrowsAsync<Exception>(() => controller.AddRoute(dto));
        }

        [Fact]
        public async Task GetRoutes_ReturnsOk_WithRoutes()
        {
            var mockService = new Mock<IRouteService>();
            var routes = new List<RouteDto>
            {
                GetRouteDto(),
                new() { Id = 2, VehicleId = "V2", Path = "B->C", Distance = 5.0, CalculatedAt = DateTime.UtcNow }
            };
            mockService.Setup(s => s.GetRoutesAsync()).ReturnsAsync(routes);

            var controller = new RouteController(mockService.Object);

            var result = await controller.GetRoutes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(routes, okResult.Value);
        }

        [Fact]
        public async Task GetRoutes_ThrowsException_WhenServiceFails()
        {
            var mockService = new Mock<IRouteService>();
            mockService.Setup(s => s.GetRoutesAsync()).ThrowsAsync(new Exception("Error"));

            var controller = new RouteController(mockService.Object);

            await Assert.ThrowsAsync<Exception>(() => controller.GetRoutes());
        }
    }
}