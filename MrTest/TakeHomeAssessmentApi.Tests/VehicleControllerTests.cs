using TakeHomeAssessmentApi.Controllers;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class VehicleControllerTests
    {
        [Fact]
        public async Task DeleteVehicle_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributedAsync("123"))
                       .ReturnsAsync(true);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = await controller.DeleteVehicle("123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Vehículo eliminado correctamente.", okResult.Value);
        }

        [Fact]
        public async Task DeleteVehicle_ReturnsStatus500_WhenDeletionFails()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributedAsync("123"))
                       .ReturnsAsync(false);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = await controller.DeleteVehicle("123");

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("No se pudo eliminar el vehículo.", statusResult.Value);
        }

        [Fact]
        public async Task DeleteVehicle_ReturnsStatus500_WhenExceptionIsThrown()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributedAsync("123"))
                       .ThrowsAsync(new Exception("Error de base de datos"));

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = await controller.DeleteVehicle("123");

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Error: Error de base de datos", statusResult.Value?.ToString() ?? string.Empty);
        }
    }
}