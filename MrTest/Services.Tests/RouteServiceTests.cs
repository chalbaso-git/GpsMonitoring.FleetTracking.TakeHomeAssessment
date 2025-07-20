using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Moq;
using Services.Implementations;
using Xunit;

namespace MrTest.Services.Tests
{
    public class RouteServiceTests
    {
        [Fact]
        public async Task AddRouteAsync_CallsRepositoryAddAsync_WithMappedEntity()
        {
            var mockRepo = new Mock<IRouteRepository>();
            var dto = new RouteDto
            {
                Id = 0,
                VehicleId = "V1",
                Path = "A->B",
                Distance = 10.5,
                CalculatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Route>())).Returns(Task.CompletedTask);

            var service = new RouteService(mockRepo.Object);

            await service.AddRouteAsync(dto);

            mockRepo.Verify(r => r.AddAsync(It.Is<Route>(route =>
                route.VehicleId == dto.VehicleId &&
                route.Path == dto.Path &&
                route.Distance == dto.Distance &&
                route.CalculatedAt == dto.CalculatedAt
            )), Times.Once);
        }

        [Fact]
        public async Task AddRouteAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IRouteRepository>();
            var dto = new RouteDto
            {
                Id = 0,
                VehicleId = "V1",
                Path = "A->B",
                Distance = 10.5,
                CalculatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Route>())).ThrowsAsync(new Exception("Repo error"));

            var service = new RouteService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddRouteAsync(dto));
            Assert.Contains("Error al agregar la ruta.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }

        [Fact]
        public async Task GetRoutesAsync_ReturnsMappedDtos_WhenRepositoryReturnsRoutes()
        {
            var mockRepo = new Mock<IRouteRepository>();
            var routes = new List<Route>
        {
            new() { Id = 1, VehicleId = "V1", Path = "A->B", Distance = 10.5, CalculatedAt = DateTime.UtcNow },
            new() { Id = 2, VehicleId = "V2", Path = "B->C", Distance = 5.0, CalculatedAt = DateTime.UtcNow }
        };

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(routes);

            var service = new RouteService(mockRepo.Object);

            var result = await service.GetRoutesAsync();

            Assert.Equal(routes.Count, result.Count);
            Assert.Equal(routes[0].Id, result[0].Id);
            Assert.Equal(routes[1].VehicleId, result[1].VehicleId);
        }

        [Fact]
        public async Task GetRoutesAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IRouteRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Repo error"));

            var service = new RouteService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetRoutesAsync());
            Assert.Contains("Error al obtener las rutas.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }
    }
}