using Services.Implementations;
using Interfaces.Infrastructure.EF;
using Cross.Dtos;
using Domain.Entities;
using Moq;
using Xunit;

namespace MrTest.Services.Tests
{
    public class AuditServiceTests
    {
        [Fact]
        public async Task LogAsync_CallsRepositorySaveAsync_WithMappedEntity()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var dto = new AuditLogDto
            {
                VehicleId = "V1",
                EventType = "TestEvent",
                Details = "Detalle",
                Timestamp = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.SaveAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            var service = new AuditService(mockRepo.Object);

            await service.LogAsync(dto);

            mockRepo.Verify(r => r.SaveAsync(It.Is<AuditLog>(log =>
                log.VehicleId == dto.VehicleId &&
                log.EventType == dto.EventType &&
                log.Details == dto.Details &&
                log.Timestamp == dto.Timestamp
            )), Times.Once);
        }

        [Fact]
        public async Task LogAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var dto = new AuditLogDto
            {
                VehicleId = "V1",
                EventType = "TestEvent",
                Details = "Detalle",
                Timestamp = DateTime.UtcNow
            };

            mockRepo.Setup(r => r.SaveAsync(It.IsAny<AuditLog>())).ThrowsAsync(new Exception("Repo error"));

            var service = new AuditService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.LogAsync(dto));
            Assert.Contains("Error al registrar el log de auditoría.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }

        [Fact]
        public async Task GetLogsAsync_ReturnsMappedDtos_WhenRepositoryReturnsLogs()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var logs = new List<AuditLog>
        {
            new() { Id = 1, VehicleId = "V1", EventType = "E1", Details = "D1", Timestamp = DateTime.UtcNow },
            new() { Id = 2, VehicleId = "V1", EventType = "E2", Details = "D2", Timestamp = DateTime.UtcNow }
        };

            mockRepo.Setup(r => r.GetByVehicleIdAsync("V1")).ReturnsAsync(logs);

            var service = new AuditService(mockRepo.Object);

            var result = await service.GetLogsAsync("V1");

            Assert.Equal(logs.Count, result.Count);
            Assert.Equal(logs[0].VehicleId, result[0].VehicleId);
            Assert.Equal(logs[1].EventType, result[1].EventType);
        }

        [Fact]
        public async Task GetLogsAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            mockRepo.Setup(r => r.GetByVehicleIdAsync("V1")).ThrowsAsync(new Exception("Repo error"));

            var service = new AuditService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLogsAsync("V1"));
            Assert.Contains("Error al obtener los logs de auditoría.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }
    }
}