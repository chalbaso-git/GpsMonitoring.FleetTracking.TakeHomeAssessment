using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using TakeHomeAssessmentApi.Controllers;
using Xunit;

namespace MrTest.TakeHomeAssessmentApi.Tests
{
    public class RoutingControllerTests
    {
        private static RouteRequestDto GetRequest() => new()
        {
            VehicleId = "V1",
            Origin = "A",
            Destination = "B"
        };

        [Fact]
        public async Task CalculateRoute_Returns503_WhenCircuitBreakerIsOpen()
        {
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();
            mockBreaker.SetupGet(b => b.IsOpen).Returns(true);

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var result = await controller.CalculateRoute(GetRequest());

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(503, statusResult.StatusCode);
            Assert.Equal("Servicio de ruteo desactivado por fallos consecutivos.", statusResult.Value);
            mockAudit.Verify(a => a.LogAsync(It.Is<AuditLogDto>(l => l.EventType == "CircuitBreakerOpen")), Times.Once);
        }

        [Fact]
        public async Task CalculateRoute_ReturnsOk_WhenRouteIsCalculated()
        {
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();
            mockBreaker.SetupGet(b => b.IsOpen).Returns(false);

            var response = new RouteResponseDto
            {
                VehicleId = "V1",
                Path = ["A", "B"],
                Distance = 10,
                CalculatedAt = DateTime.UtcNow
            };
            mockRouting.Setup(r => r.CalculateRouteAsync(It.IsAny<RouteRequestDto>())).ReturnsAsync(response);

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var result = await controller.CalculateRoute(GetRequest());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
            mockBreaker.Verify(b => b.Reset(), Times.Once);
            mockAudit.Verify(a => a.LogAsync(It.Is<AuditLogDto>(l => l.EventType == "RouteCalculated")), Times.Once);
        }

        [Fact]
        public async Task CalculateRoute_ReturnsBadRequest_OnArgumentException()
        {
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();
            mockBreaker.SetupGet(b => b.IsOpen).Returns(false);

            mockRouting.Setup(r => r.CalculateRouteAsync(It.IsAny<RouteRequestDto>()))
                .ThrowsAsync(new ArgumentException("Datos inválidos"));

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var result = await controller.CalculateRoute(GetRequest());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Datos inválidos", badRequest.Value);
            mockBreaker.Verify(b => b.RegisterFailure(), Times.Once);
            mockAudit.Verify(a => a.LogAsync(It.Is<AuditLogDto>(l => l.EventType == "CircuitBreakerActivated")), Times.Never);
        }


        [Fact]
        public async Task ResetCircuit_ResetsBreakerAndLogs()
        {
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var result = await controller.ResetCircuit();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Circuito reiniciado.", okResult.Value);
            mockBreaker.Verify(b => b.Reset(), Times.Once);
            mockAudit.Verify(a => a.LogAsync(It.Is<AuditLogDto>(l => l.EventType == "CircuitBreakerReset")), Times.Once);
        }

        [Fact]
        public async Task CalculateRoute_ReturnsStatus500_OnGeneralException()
        {
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();
            mockBreaker.SetupGet(b => b.IsOpen).Returns(false);

            // Simula excepción general
            mockRouting.Setup(r => r.CalculateRouteAsync(It.IsAny<RouteRequestDto>()))
                .ThrowsAsync(new Exception("Error inesperado"));

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var result = await controller.CalculateRoute(GetRequest());

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Error interno: Error inesperado", statusResult.Value);
            mockBreaker.Verify(b => b.RegisterFailure(), Times.Once);
            mockAudit.Verify(a => a.LogAsync(It.Is<AuditLogDto>(l => l.EventType == "CircuitBreakerActivated")), Times.Never);
        }

        [Fact]
        public async Task HandleFailure_LogsCircuitBreakerActivated_WhenBreakerIsOpen()
        {
            // Arrange
            var mockRouting = new Mock<IRoutingService>();
            var mockAudit = new Mock<IAuditService>();
            var mockBreaker = new Mock<ICircuitBreakerService>();
            mockBreaker.Setup(b => b.RegisterFailure());
            mockBreaker.SetupGet(b => b.IsOpen).Returns(true);

            var controller = new RoutingController(mockRouting.Object, mockAudit.Object, mockBreaker.Object);

            var logCalled = false;
            var expectedVehicleId = "V1";
            var expectedEventType = "CircuitBreakerActivated";
            var expectedDetails = "Circuit Breaker activado por 3 fallos consecutivos. Detalle: Test details";

            var logMethod = controller.GetType()
                .GetMethod("LogCircuitBreakerEvent", BindingFlags.NonPublic | BindingFlags.Instance);

            controller.GetType()
                .GetField("_auditService", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(controller, mockAudit.Object);

            mockAudit.Setup(a => a.LogAsync(It.Is<AuditLogDto>(l =>
                l.VehicleId == expectedVehicleId &&
                l.EventType == expectedEventType &&
                l.Details == expectedDetails
            ))).Callback(() => logCalled = true).Returns(Task.CompletedTask);



            var method = controller.GetType()
               .GetMethod("HandleFailure", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method is not null)
            {
                var task = method.Invoke(controller, new object[] { expectedVehicleId, "Test details" }) as Task;
                if (task is not null)
                {
                    await task;
                }       
            }

            // Assert
            mockBreaker.Verify(b => b.RegisterFailure(), Times.Once);
            Assert.True(logCalled);
        }
    }
}