using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Moq;
using Services.Implementations;
using Xunit;

namespace MrTest.Services.Tests
{
    public class AlertServiceTests
    {
        [Fact]
        public async Task AddAlertAsync_CallsRepositoryAddAsync_WithMappedEntity()
        {
            var mockRepo = new Mock<IAlertRepository>();
            var dto = new AlertDto
            {
                Id = 0,
                VehicleId = "V1",
                Type = "Warning",
                Message = "Test alert",
                CreatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Alert>())).Returns(Task.CompletedTask);

            var service = new AlertService(mockRepo.Object);

            await service.AddAlertAsync(dto);

            mockRepo.Verify(r => r.AddAsync(It.Is<Alert>(alert =>
                alert.VehicleId == dto.VehicleId &&
                alert.Type == dto.Type &&
                alert.Message == dto.Message &&
                alert.CreatedAt == dto.CreatedAt
            )), Times.Once);
        }

        [Fact]
        public async Task AddAlertAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAlertRepository>();
            var dto = new AlertDto
            {
                Id = 0,
                VehicleId = "V1",
                Type = "Warning",
                Message = "Test alert",
                CreatedAt = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Alert>())).ThrowsAsync(new Exception("Repo error"));

            var service = new AlertService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAlertAsync(dto));
            Assert.Contains("Error al agregar la alerta.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }

        [Fact]
        public async Task GetAlertsAsync_ReturnsMappedDtos_WhenRepositoryReturnsAlerts()
        {
            var mockRepo = new Mock<IAlertRepository>();
            var alerts = new List<Alert>
        {
            new() { Id = 1, VehicleId = "V1", Type = "Warning", Message = "Test alert", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, VehicleId = "V2", Type = "Info", Message = "Another alert", CreatedAt = DateTime.UtcNow }
        };

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(alerts);

            var service = new AlertService(mockRepo.Object);

            var result = await service.GetAlertsAsync();

            Assert.Equal(alerts.Count, result.Count);
            Assert.Equal(alerts[0].Id, result[0].Id);
            Assert.Equal(alerts[1].VehicleId, result[1].VehicleId);
            Assert.Equal(alerts[1].Type, result[1].Type);
        }

        [Fact]
        public async Task GetAlertsAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAlertRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Repo error"));

            var service = new AlertService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAlertsAsync());
            Assert.Contains("Error al obtener las alertas.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }
    }
}