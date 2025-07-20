using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TakeHomeAssessmentApi.Controllers;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class GeolocationControllerTests
    {
        private static GpsCoordinateDto GetValidCoordinate() => new()
        {
            VehicleId = "V1",
            Latitude = 40.4168,
            Longitude = -3.7038,
            Timestamp = DateTime.UtcNow
        };

        [Fact]
        public async Task StoreCoordinate_ReturnsBadRequest_WhenCoordinateIsNull()
        {
            var mockService = new Mock<IGeolocationService>();
            var controller = new GeolocationController(mockService.Object);

            var result = await controller.StoreCoordinate(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid GPS coordinate data.", badRequest.Value);
        }

        [Fact]
        public async Task StoreCoordinate_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IGeolocationService>();
            mockService.Setup(s => s.StoreCoordinateAsync(It.IsAny<GpsCoordinateDto>()))
                       .Returns(Task.CompletedTask);

            var controller = new GeolocationController(mockService.Object);

            var result = await controller.StoreCoordinate(GetValidCoordinate());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("GPS coordinate stored successfully.", okResult.Value);
        }

        [Fact]
        public async Task StoreCoordinate_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            var mockService = new Mock<IGeolocationService>();
            mockService.Setup(s => s.StoreCoordinateAsync(It.IsAny<GpsCoordinateDto>()))
                       .ThrowsAsync(new ArgumentException("Datos inválidos"));

            var controller = new GeolocationController(mockService.Object);

            var result = await controller.StoreCoordinate(GetValidCoordinate());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Datos inválidos", badRequest.Value);
        }

        [Fact]
        public async Task StoreCoordinate_ReturnsStatus500_WhenGeneralExceptionThrown()
        {
            var mockService = new Mock<IGeolocationService>();
            mockService.Setup(s => s.StoreCoordinateAsync(It.IsAny<GpsCoordinateDto>()))
                       .ThrowsAsync(new Exception("Error inesperado"));

            var controller = new GeolocationController(mockService.Object);

            var result = await controller.StoreCoordinate(GetValidCoordinate());

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("An error occurred while storing the GPS coordinate: Error inesperado", statusResult.Value?.ToString() ?? string.Empty);
        }
    }
}