using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TakeHomeAssessmentApi.Controllers;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class VehicleControllerTests
    {
        [Fact]
        public void DeleteVehicle_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributed("123")).Returns(true);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.DeleteVehicle("123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Vehículo eliminado correctamente.", okResult.Value);
        }

        [Fact]
        public void DeleteVehicle_ReturnsStatus500_WhenDeletionFails()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributed("123")).Returns(false);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.DeleteVehicle("123");

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("No se pudo eliminar el vehículo.", statusResult.Value);
        }

        [Fact]
        public void DeleteVehicle_ReturnsStatus500_WhenExceptionIsThrown()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.DeleteVehicleDistributed("123"))
                       .Throws(new Exception("Error de base de datos"));

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.DeleteVehicle("123");

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Contains("Error: Error de base de datos", statusResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public void GetVehicles_ReturnsListOfVehicles()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            var vehicles = new List<VehicleDto>
            {
                new() { Id = "1", Name = "Vehículo 1" },
                new() { Id = "2", Name = "Vehículo 2" }
            };
            mockService.Setup(s => s.GetVehicles()).Returns(vehicles);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.GetVehicles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicles = Assert.IsType<List<VehicleDto>>(okResult.Value);
            Assert.Equal(2, returnedVehicles.Count);
        }

        [Fact]
        public void GetVehicleById_ReturnsVehicle_WhenExists()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            var vehicle = new VehicleDto { Id = "1", Name = "Vehículo 1" };
            mockService.Setup(s => s.GetVehicleById("1")).Returns(vehicle);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.GetVehicleById("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVehicle = Assert.IsType<VehicleDto>(okResult.Value);
            Assert.Equal("1", returnedVehicle.Id);
        }

        [Fact]
        public void GetVehicleById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            mockService.Setup(s => s.GetVehicleById("1")).Returns((VehicleDto)null!);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.GetVehicleById("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateVehicle_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            var vehicleDto = new VehicleDto { Id = "1", Name = "Vehículo 1" };
            mockService.Setup(s => s.UpdateVehicle(vehicleDto)).Returns(true);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.UpdateVehicle("1", vehicleDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value!);
        }

        [Fact]
        public void UpdateVehicle_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            var vehicleDto = new VehicleDto { Id = "2", Name = "Vehículo 2" };

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.UpdateVehicle("1", vehicleDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El id de la URL no coincide con el del cuerpo de la solicitud.", badRequest.Value);
        }

        [Fact]
        public void UpdateVehicle_ReturnsNotFound_WhenVehicleDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IVehicleService>();
            var vehicleDto = new VehicleDto { Id = "1", Name = "Vehículo 1" };
            mockService.Setup(s => s.UpdateVehicle(vehicleDto)).Returns(false);

            var controller = new VehicleController(mockService.Object);

            // Act
            var result = controller.UpdateVehicle("1", vehicleDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}