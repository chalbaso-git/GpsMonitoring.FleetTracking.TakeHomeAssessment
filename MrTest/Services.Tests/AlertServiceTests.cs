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
    public class AlertServiceTests
    {
        [Fact]
        public void AddAlertAsync_CallsRepositoryAddAsync_WithMappedEntity()
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

            mockRepo
                .Setup(repo => repo.Add(It.IsAny<Alert>()));

            var service = new AlertService(mockRepo.Object);

            service.AddAlert(dto);

            mockRepo.Verify(r => r.Add(It.Is<Alert>(alert =>
                alert.VehicleId == dto.VehicleId &&
                alert.Type == dto.Type &&
                alert.Message == dto.Message &&
                alert.CreatedAt == dto.CreatedAt
            )), Times.Once);
        }

        [Fact]
        public void AddAlertAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
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

            mockRepo
                .Setup(repo => repo.Add(It.IsAny<Alert>()))
                .Throws(new Exception("Repo error"));

            var service = new AlertService(mockRepo.Object);

            // Fix: Use Assert.Throws<InvalidOperationException> for synchronous method
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddAlert(dto));
            Assert.Contains("Error al agregar la alerta.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }

        [Fact]
        public void GetAlertsAsync_ReturnsMappedDtos_WhenRepositoryReturnsAlerts()
        {
            var mockRepo = new Mock<IAlertRepository>();
            var alerts = new List<Alert>
            {
                new() { Id = 1, VehicleId = "V1", Type = "Warning", Message = "Test alert", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, VehicleId = "V2", Type = "Info", Message = "Another alert", CreatedAt = DateTime.UtcNow }
            };

            mockRepo
                .Setup(repo => repo.Find(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<FindOption>()))
                .Returns(alerts);

            var service = new AlertService(mockRepo.Object);

            var result = service.GetAlerts();

            Assert.Equal(alerts.Count, result.Count);
            Assert.Equal(alerts[0].Id, result[0].Id);
            Assert.Equal(alerts[1].VehicleId, result[1].VehicleId);
            Assert.Equal(alerts[1].Type, result[1].Type);
        }

        [Fact]
        public void GetAlerts_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            var mockRepo = new Mock<IAlertRepository>();

            mockRepo
                .Setup(repo => repo.Find(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<FindOption>()))
                .Throws(new Exception("Repo error"));

            var service = new AlertService(mockRepo.Object);

            var ex = Assert.Throws<InvalidOperationException>(() => service.GetAlerts());
            Assert.Contains("Error al obtener las alertas.", ex.Message);
            Assert.Equal("Repo error", ex.InnerException!.Message);
        }
    }
}