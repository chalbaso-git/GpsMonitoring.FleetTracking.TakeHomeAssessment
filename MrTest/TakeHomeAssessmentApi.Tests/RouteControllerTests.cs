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
        public void GetRoutes_ReturnsOk_WithRoutes()
        {
            var mockService = new Mock<IRouteService>();
            var routes = new List<RouteDto>
            {
                GetRouteDto(),
                new() { Id = 2, VehicleId = "V2", Path = "B->C", Distance = 5.0, CalculatedAt = DateTime.UtcNow }
            };
            mockService.Setup(s => s.GetRoutes()).Returns(routes);

            var controller = new RouteController(mockService.Object);

            var result = controller.GetRoutes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(routes, okResult.Value);
        }

        [Fact]
        public async Task GetRoutes_ThrowsException_WhenServiceFails()
        {
            var mockService = new Mock<IRouteService>();
            mockService.Setup(s => s.GetRoutes()).Throws(new Exception("Error"));

            var controller = new RouteController(mockService.Object);

            await Assert.ThrowsAsync<Exception>(() => Task.Run(() => controller.GetRoutes()));
        }

        [Fact]
        public void GetRoutesByVehicleAndDate_ReturnsOkWithFilteredRoutes()
        {
            // Arrange
            var mockService = new Mock<IRouteService>();
            var vehicleId = "V1";
            var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            var expectedRoutes = new List<RouteDto>
            {
                new() { Id = 1, VehicleId = vehicleId, Path = "A,B", Distance = 10, CalculatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc) },
                new() { Id = 2, VehicleId = vehicleId, Path = "B,C", Distance = 15, CalculatedAt = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc) }
            };

            mockService.Setup(s => s.GetRoutesByVehicleAndDate(vehicleId, from, to)).Returns(expectedRoutes);

            var controller = new RouteController(mockService.Object);

            // Act
            var result = controller.GetRoutesByVehicleAndDate(vehicleId, from, to);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualRoutes = Assert.IsType<List<RouteDto>>(okResult.Value);
            Assert.Equal(expectedRoutes.Count, actualRoutes.Count);
            Assert.All(actualRoutes, r => Assert.Equal(vehicleId, r.VehicleId));
        }
    }
}