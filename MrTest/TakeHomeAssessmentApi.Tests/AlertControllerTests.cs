using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TakeHomeAssessmentApi.Controllers;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class AlertControllerTests
    {
        private static AlertDto GetAlertDto() => new()
        {
            Id = 1,
            VehicleId = "V1",
            Type = "Warning",
            Message = "Test alert",
            CreatedAt = DateTime.UtcNow
        };

        [Fact]
        public void AddAlert_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IAlertService>();
            var dto = GetAlertDto();
            mockService.Setup(s => s.AddAlert(dto));

            var controller = new AlertController(mockService.Object);

            var result = controller.AddAlert(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Alerta registrada.", okResult.Value);
        }

        [Fact]
        public void AddAlert_ThrowsException_WhenServiceFails()
        {
            var mockService = new Mock<IAlertService>();
            var dto = GetAlertDto();
            mockService.Setup(s => s.AddAlert(dto)).Throws(new Exception("Error"));

            var controller = new AlertController(mockService.Object);

            Assert.Throws<Exception>(() => controller.AddAlert(dto));
        }

        [Fact]
        public void GetAlerts_ReturnsOk_WithAlerts()
        {
            var mockService = new Mock<IAlertService>();
            var alerts = new List<AlertDto>
            {
                GetAlertDto(),
                new() { Id = 2, VehicleId = "V2", Type = "Info", Message = "Another alert", CreatedAt = DateTime.UtcNow }
            };
            mockService.Setup(s => s.GetAlerts()).Returns(alerts);

            var controller = new AlertController(mockService.Object);

            var result = controller.GetAlerts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(alerts, okResult.Value);
        }

        [Fact]
        public void GetAlerts_ThrowsException_WhenServiceFails()
        {
            var mockService = new Mock<IAlertService>();
            mockService.Setup(s => s.GetAlerts()).Throws(new Exception("Error"));

            var controller = new AlertController(mockService.Object);

            Assert.Throws<Exception>(() => controller.GetAlerts());
        }
    }
}