using Cross.Dtos;
using Cross.Helpers.Context;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Moq;
using Services.Implementations;
using System.Linq.Expressions;
using Xunit;

namespace MrTest.Services.Tests
{
    public class AuditServiceTests
    {
        [Fact]
        public void LogAsync_CallsRepositorySaveAsync_WithMappedEntity()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var dto = new AuditLogDto
            {
                VehicleId = "V1",
                EventType = "TestEvent",
                Details = "Detalle",
                Timestamp = DateTime.UtcNow
            };

            mockRepo
                .Setup(repo => repo.Add(It.IsAny<AuditLog>()));

            var service = new AuditService(mockRepo.Object);

            service.Log(dto);

            mockRepo.Verify(r => r.Add(It.Is<AuditLog>(log =>
                log.VehicleId == dto.VehicleId &&
                log.EventType == dto.EventType &&
                log.Details == dto.Details &&
                log.Timestamp == dto.Timestamp
            )), Times.Once);
        }

        [Fact]
        public void LogAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var dto = new AuditLogDto
            {
                VehicleId = "V1",
                EventType = "TestEvent",
                Details = "Detalle",
                Timestamp = DateTime.UtcNow
            };

            mockRepo
                .Setup(repo => repo.Add(It.IsAny<AuditLog>()))
                .Throws(new Exception("Repo error"));

            var service = new AuditService(mockRepo.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => service.Log(dto));
            Assert.Contains("Error al registrar el log de auditoría.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }

        [Fact]
        public void GetLogsAsync_ReturnsMappedDtos_WhenRepositoryReturnsLogs()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            var logs = new List<AuditLog>
            {
                new() { Id = 1, VehicleId = "V1", EventType = "E1", Details = "D1", Timestamp = DateTime.UtcNow },
                new() { Id = 2, VehicleId = "V1", EventType = "E2", Details = "D2", Timestamp = DateTime.UtcNow }
            };

            mockRepo
                .Setup(repo => repo.Find(It.IsAny<Expression<Func<AuditLog, bool>>>(), It.IsAny<FindOption>()))
                .Returns(logs);

            var service = new AuditService(mockRepo.Object);

            var result = service.GetLogs("V1");

            Assert.Equal(logs.Count, result.Count);
            Assert.Equal(logs[0].VehicleId, result[0].VehicleId);
            Assert.Equal(logs[1].EventType, result[1].EventType);
        }

        [Fact]
        public void GetLogsAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAuditLogRepository>();
            
            mockRepo
                .Setup(repo => repo.Find(It.IsAny<Expression<Func<AuditLog, bool>>>(), It.IsAny<FindOption>()))
                .Throws(new Exception("Repo error"));

            var service = new AuditService(mockRepo.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => service.GetLogs("V1"));
            Assert.Contains("Error al obtener los logs de auditoría.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }
    }
}