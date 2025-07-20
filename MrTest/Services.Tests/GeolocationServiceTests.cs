using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.Redis;
using Moq;
using Services.Implementations;
using System.Reflection;
using Xunit;

namespace MrTest.Services.Tests
{
    public class GeolocationServiceTests
    {
        private static GpsCoordinateDto GetValidDto() => new()
        {
            VehicleId = "V1",
            Latitude = 40.4168,
            Longitude = -3.7038,
            Timestamp = DateTime.UtcNow
        };

        [Fact]
        public async Task StoreCoordinateAsync_SavesCoordinate_WhenNoLastCoordinate()
        {
            var mockRedis = new Mock<IRedisClient>();
            mockRedis.Setup(r => r.GetLastCoordinateAsync("V1")).ReturnsAsync((GpsCoordinate)null!);
            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).Returns(Task.CompletedTask);

            var service = new GeolocationService(mockRedis.Object);

            await service.StoreCoordinateAsync(GetValidDto());

            mockRedis.Verify(r => r.SaveCoordinateAsync(It.Is<GpsCoordinate>(c => c.VehicleId == "V1")), Times.Once);
        }

        [Fact]
        public async Task StoreCoordinateAsync_DoesNotSave_WhenDuplicate()
        {
            var mockRedis = new Mock<IRedisClient>();
            var dto = GetValidDto();
            var last = new GpsCoordinate
            {
                VehicleId = dto.VehicleId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timestamp = dto.Timestamp // Igualar el timestamp
            };
            mockRedis.Setup(r => r.GetLastCoordinateAsync(dto.VehicleId)).ReturnsAsync(last);

            var service = new GeolocationService(mockRedis.Object);

            await service.StoreCoordinateAsync(dto);

            mockRedis.Verify(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>()), Times.Never);
        }

        [Fact]
        public async Task StoreCoordinateAsync_ThrowsInvalidOperationException_WhenRedisThrows_OnSave()
        {
            var mockRedis = new Mock<IRedisClient>();
            mockRedis.Setup(r => r.GetLastCoordinateAsync("V1")).ReturnsAsync((GpsCoordinate)null!);
            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).ThrowsAsync(new Exception("Redis error"));

            var service = new GeolocationService(mockRedis.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.StoreCoordinateAsync(GetValidDto()));
            Assert.Contains("Error al almacenar la coordenada GPS.", ex.Message);
            Assert.Equal("Redis error", ex.InnerException!.Message);
        }

        [Fact]
        public void ValidateCoordinate_ShouldNotThrow_WhenDtoIsValid()
        {
            // Arrange
            var validDto = new GpsCoordinateDto
            {
                VehicleId = "V1",
                Latitude = 40.4168,
                Longitude = -3.7038,
                Timestamp = DateTime.UtcNow
            };

            var method = typeof(GeolocationService)
                .GetMethod("ValidateCoordinate", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            Exception? ex = Record.Exception(() => method!.Invoke(null, [validDto]));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ValidateCoordinate_ShouldThrowArgumentException_WhenDtoIsOutOfRange()
        {
            // Arrange
            var invalidDto = new GpsCoordinateDto
            {
                VehicleId = "V1",
                Latitude = 100,
                Longitude = 0,
                Timestamp = DateTime.UtcNow
            };

            var method = typeof(GeolocationService)
                .GetMethod("ValidateCoordinate", BindingFlags.NonPublic | BindingFlags.Static);

            // Act & Assert
            var ex = Assert.Throws<TargetInvocationException>(() => method!.Invoke(null, [invalidDto]));
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Equal("Invalid GPS coordinates.", ex.InnerException!.Message);
        }

        [Fact]
        public async Task ProcessPendingQueueAsync_SavesAllCoordinates_WhenRedisIsAvailable()
        {
            var mockRedis = new Mock<IRedisClient>();
            var service = new GeolocationService(mockRedis.Object);

            // Accede a la cola privada usando reflexión
            var queueField = typeof(GeolocationService)
                .GetField("_pendingQueue", BindingFlags.Static | BindingFlags.NonPublic);

            var queue = (System.Collections.Concurrent.ConcurrentQueue<GpsCoordinate>)queueField!.GetValue(null)!;
            queue.Clear();

            var coord1 = new GpsCoordinate { VehicleId = "V1", Latitude = 1, Longitude = 2, Timestamp = DateTime.UtcNow };
            var coord2 = new GpsCoordinate { VehicleId = "V2", Latitude = 3, Longitude = 4, Timestamp = DateTime.UtcNow };
            queue.Enqueue(coord1);
            queue.Enqueue(coord2);

            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).Returns(Task.CompletedTask);

            await service.ProcessPendingQueueAsync();

            // Verifica que ambos se guardaron y la cola está vacía
            mockRedis.Verify(r => r.SaveCoordinateAsync(coord1), Times.Once);
            mockRedis.Verify(r => r.SaveCoordinateAsync(coord2), Times.Once);
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public async Task ProcessPendingQueueAsync_ReEnqueues_WhenRedisFails()
        {
            var mockRedis = new Mock<IRedisClient>();
            var service = new GeolocationService(mockRedis.Object);

            // Accede a la cola privada usando reflexión
            var queueField = typeof(GeolocationService)
                .GetField("_pendingQueue", BindingFlags.Static | BindingFlags.NonPublic);

            var queue = (System.Collections.Concurrent.ConcurrentQueue<GpsCoordinate>)queueField!.GetValue(null)!;
            queue.Clear();

            var coord1 = new GpsCoordinate { VehicleId = "V1", Latitude = 1, Longitude = 2, Timestamp = DateTime.UtcNow };
            queue.Enqueue(coord1);

            // Simula fallo al guardar
            mockRedis.Setup(r => r.SaveCoordinateAsync(coord1)).ThrowsAsync(new Exception("Redis error"));

            await service.ProcessPendingQueueAsync();

            // Verifica que se intentó guardar y que la coordenada sigue en la cola
            mockRedis.Verify(r => r.SaveCoordinateAsync(coord1), Times.Once);
            Assert.False(queue.IsEmpty);
        }


    }
}