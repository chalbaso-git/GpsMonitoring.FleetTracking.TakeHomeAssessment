using Services.Implementations;
using Interfaces.Infrastructure;
using Interfaces.Infrastructure.EF;
using Interfaces.Services;
using Cross.Dtos;
using Domain.Entities;
using Moq;
using Xunit;

namespace MrTest.Services.Tests
{
    public class RoutingServiceTests
    {
        private static RouteRequestDto GetRequest() => new()
        {
            VehicleId = "V1",
            Origin = "A",
            Destination = "B"
        };

        [Fact]
        public async Task CalculateRouteAsync_ReturnsCachedRoute_WhenExists()
        {
            var cachedRoute = new Route
            {
                Id = 1,
                VehicleId = "V1",
                Path = "A,X,B",
                Distance = 12.5,
                CalculatedAt = DateTime.UtcNow
            };

            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync(cachedRoute);

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var result = await service.CalculateRouteAsync(GetRequest());

            Assert.Equal("V1", result.VehicleId);
            Assert.Equal(["A", "X", "B"], result.Path);
            mockRouteService.Verify(r => r.AddRouteAsync(It.IsAny<RouteDto>()), Times.Once);
        }

        [Fact]
        public async Task CalculateRouteAsync_ThrowsArgumentException_WhenOriginOrDestinationMissing()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var badRequest = new RouteRequestDto { VehicleId = "V1", Origin = "", Destination = "" };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateRouteAsync(badRequest));
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Contains("Error en el servicio de ruteo.", ex.Message);
        }

        [Fact]
        public async Task CalculateRouteAsync_ThrowsInvalidOperationException_WhenDeadlock()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync((Route)null!);
            mockCache.Setup(c => c.AcquireZoneLockAsync("A", "B", "V1", It.IsAny<TimeSpan>())).ReturnsAsync(false);

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateRouteAsync(GetRequest()));
            Assert.Contains("Zona ocupada", ex.InnerException!.Message);
            mockAlertService.Verify(a => a.AddAlertAsync(It.Is<AlertDto>(d => d.Type == "Deadlock")), Times.Once);
        }

        [Fact]
        public async Task CalculateRouteAsync_ThrowsInvalidOperationException_WhenUnexpectedException()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync((Route)null!);
            mockCache.Setup(c => c.AcquireZoneLockAsync("A", "B", "V1", It.IsAny<TimeSpan>())).ReturnsAsync(true);
            mockWaypointRepo.Setup(w => w.GetAllAsync()).ThrowsAsync(new Exception("Repo error"));

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateRouteAsync(GetRequest()));
            Assert.Contains("Error en el servicio de ruteo.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
            mockAlertService.Verify(a => a.AddAlertAsync(It.Is<AlertDto>(d => d.Type == "Error")), Times.Once);
        }

        [Fact]
        public async Task ValidateRequest_ThrowsArgumentException_WhenOriginOrDestinationIsEmpty()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var badRequest = new RouteRequestDto { VehicleId = "V1", Origin = "", Destination = "" };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateRouteAsync(badRequest));
            Assert.IsType<ArgumentException>(ex.InnerException);
        }

        [Fact]
        public async Task HandleCachedRoute_IsCalled_WhenCachedRouteExists()
        {
            var cachedRoute = new Route
            {
                Id = 1,
                VehicleId = "V1",
                Path = "A,X,B",
                Distance = 12.5,
                CalculatedAt = DateTime.UtcNow
            };

            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync(cachedRoute);

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var result = await service.CalculateRouteAsync(new RouteRequestDto { VehicleId = "V1", Origin = "A", Destination = "B" });

            Assert.Equal("V1", result.VehicleId);
            Assert.Equal(["A", "X", "B"], result.Path);
            mockRouteService.Verify(r => r.AddRouteAsync(It.IsAny<RouteDto>()), Times.Once);
        }

        [Fact]
        public async Task HandleDeadlock_IsCalled_WhenLockNotAcquired()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync((Route)null!);
            mockCache.Setup(c => c.AcquireZoneLockAsync("A", "B", "V1", It.IsAny<TimeSpan>())).ReturnsAsync(false);

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateRouteAsync(new RouteRequestDto { VehicleId = "V1", Origin = "A", Destination = "B" }));
            Assert.Contains("Zona ocupada", ex.InnerException!.Message);
            mockAlertService.Verify(a => a.AddAlertAsync(It.Is<AlertDto>(d => d.Type == "Deadlock")), Times.Once);
        }

        [Fact]
        public async Task GetDynamicWaypoints_BuildPath_CreateRoute_MapToDto_MapToResponseDto_AreCovered_WhenRouteIsCalculated()
        {
            var mockCache = new Mock<IRouteCache>();
            var mockRouteService = new Mock<IRouteService>();
            var mockAlertService = new Mock<IAlertService>();
            var mockWaypointRepo = new Mock<IWaypointRepository>();

            mockCache.Setup(c => c.GetCachedRouteAsync("V1", "A", "B")).ReturnsAsync((Route)null!);
            mockCache.Setup(c => c.AcquireZoneLockAsync("A", "B", "V1", It.IsAny<TimeSpan>())).ReturnsAsync(true);
            mockCache.Setup(c => c.SaveRouteAsync(It.IsAny<Route>())).Returns(Task.CompletedTask);
            mockCache.Setup(c => c.ReleaseZoneLockAsync("A", "B", "V1")).Returns(Task.CompletedTask);

            mockRouteService.Setup(r => r.AddRouteAsync(It.IsAny<RouteDto>())).Returns(Task.CompletedTask);

            var waypoints = new List<Waypoint>
        {
            new() { Id = 1, Name = "X", Latitude = 0, Longitude = 0 },
            new() { Id = 2, Name = "Y", Latitude = 0, Longitude = 0 }
        };
            mockWaypointRepo.Setup(w => w.GetAllAsync()).ReturnsAsync(waypoints);

            var service = new RoutingService(mockCache.Object, mockRouteService.Object, mockAlertService.Object, mockWaypointRepo.Object);

            var result = await service.CalculateRouteAsync(new RouteRequestDto { VehicleId = "V1", Origin = "A", Destination = "B" });

            Assert.Equal("V1", result.VehicleId);
            Assert.Contains("A", result.Path);
            Assert.Contains("B", result.Path);
            Assert.True(result.Path.Count >= 3);
            Assert.True(result.Distance >= 10.5);
            Assert.True(result.CalculatedAt <= DateTime.UtcNow);
        }
    }
}