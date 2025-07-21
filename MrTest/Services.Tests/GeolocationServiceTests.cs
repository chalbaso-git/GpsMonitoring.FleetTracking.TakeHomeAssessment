using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.Redis;
using Interfaces.Services;
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

        private static void ClearPendingQueue()
        {
            var queueField = typeof(GeolocationService)
                .GetField("_pendingQueue", BindingFlags.Static | BindingFlags.NonPublic);

            var queue = (System.Collections.Concurrent.ConcurrentQueue<GpsCoordinate>)queueField!.GetValue(null)!;
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out _);
            }
        }

        [Fact]
        public async Task StoreCoordinateAsync_SavesCoordinate_AndUpdatesVehicle_WhenNoLastCoordinate()
        {
            ClearPendingQueue();
            var mockRedis = new Mock<IRedisClient>();
            var mockVehicleService = new Mock<IVehicleService>();
            var dto = GetValidDto();

            mockRedis.Setup(r => r.GetLastCoordinateAsync(dto.VehicleId)).ReturnsAsync((GpsCoordinate)null!);
            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).Returns(Task.CompletedTask);

            var vehicleDto = new VehicleDto { Id = dto.VehicleId };
            mockVehicleService.Setup(v => v.GetVehicleById(dto.VehicleId)).Returns(vehicleDto);
            mockVehicleService.Setup(v => v.UpdateVehicle(It.IsAny<VehicleDto>())).Returns(true);

            var service = new GeolocationService(mockRedis.Object, mockVehicleService.Object);

            await service.StoreCoordinateAsync(dto);

            mockRedis.Verify(r => r.SaveCoordinateAsync(It.Is<GpsCoordinate>(c => c.VehicleId == dto.VehicleId)), Times.Once);
            mockVehicleService.Verify(v => v.GetVehicleById(dto.VehicleId), Times.AtLeastOnce);
            mockVehicleService.Verify(v => v.UpdateVehicle(It.Is<VehicleDto>(v =>
                v.LastLocation == $"{dto.Latitude},{dto.Longitude}" &&
                v.LastSeen == dto.Timestamp
            )), Times.AtLeastOnce);
        }

        [Fact]
        public async Task StoreCoordinateAsync_DoesNotSave_WhenDuplicate()
        {
            ClearPendingQueue();
            var mockRedis = new Mock<IRedisClient>();
            var mockVehicleService = new Mock<IVehicleService>();
            var dto = GetValidDto();
            var last = new GpsCoordinate
            {
                VehicleId = dto.VehicleId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timestamp = dto.Timestamp
            };
            mockRedis.Setup(r => r.GetLastCoordinateAsync(dto.VehicleId)).ReturnsAsync(last);

            var service = new GeolocationService(mockRedis.Object, mockVehicleService.Object);

            await service.StoreCoordinateAsync(dto);

            mockRedis.Verify(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>()), Times.Never);
            mockVehicleService.Verify(v => v.UpdateVehicle(It.IsAny<VehicleDto>()), Times.Never);
        }

        [Fact]
        public async Task StoreCoordinateAsync_ThrowsInvalidOperationException_WhenRedisThrows_OnSave()
        {
            ClearPendingQueue();
            var mockRedis = new Mock<IRedisClient>();
            var mockVehicleService = new Mock<IVehicleService>();
            var dto = GetValidDto();

            mockRedis.Setup(r => r.GetLastCoordinateAsync(dto.VehicleId)).ReturnsAsync((GpsCoordinate)null!);
            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).ThrowsAsync(new Exception("Redis error"));

            var vehicleDto = new VehicleDto { Id = dto.VehicleId };
            mockVehicleService.Setup(v => v.GetVehicleById(dto.VehicleId)).Returns(vehicleDto);

            var service = new GeolocationService(mockRedis.Object, mockVehicleService.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.StoreCoordinateAsync(dto));
            Assert.Contains("Error al almacenar la coordenada GPS.", ex.Message);
            Assert.Equal("Redis error", ex.InnerException!.Message);
        }

        [Fact]
        public void ValidateCoordinate_ShouldNotThrow_WhenDtoIsValid()
        {
            var validDto = new GpsCoordinateDto
            {
                VehicleId = "V1",
                Latitude = 40.4168,
                Longitude = -3.7038,
                Timestamp = DateTime.UtcNow
            };

            var method = typeof(GeolocationService)
                .GetMethod("ValidateCoordinate", BindingFlags.NonPublic | BindingFlags.Static);

            Exception? ex = Record.Exception(() => method!.Invoke(null, [validDto]));

            Assert.Null(ex);
        }

        [Fact]
        public void ValidateCoordinate_ShouldThrowArgumentException_WhenDtoIsOutOfRange()
        {
            var invalidDto = new GpsCoordinateDto
            {
                VehicleId = "V1",
                Latitude = 100,
                Longitude = 0,
                Timestamp = DateTime.UtcNow
            };

            var method = typeof(GeolocationService)
                .GetMethod("ValidateCoordinate", BindingFlags.NonPublic | BindingFlags.Static);

            var ex = Assert.Throws<TargetInvocationException>(() => method!.Invoke(null, [invalidDto]));
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Equal("Invalid GPS coordinates.", ex.InnerException!.Message);
        }

        [Fact]
        public async Task ProcessPendingQueueAsync_SavesAllCoordinates_WhenRedisIsAvailable()
        {
            ClearPendingQueue();
            var mockRedis = new Mock<IRedisClient>();
            var mockVehicleService = new Mock<IVehicleService>();
            var service = new GeolocationService(mockRedis.Object, mockVehicleService.Object);

            var queueField = typeof(GeolocationService)
                .GetField("_pendingQueue", BindingFlags.Static | BindingFlags.NonPublic);

            var queue = (System.Collections.Concurrent.ConcurrentQueue<GpsCoordinate>)queueField!.GetValue(null)!;

            var coord1 = new GpsCoordinate { VehicleId = "V1", Latitude = 1, Longitude = 2, Timestamp = DateTime.UtcNow };
            var coord2 = new GpsCoordinate { VehicleId = "V2", Latitude = 3, Longitude = 4, Timestamp = DateTime.UtcNow };
            queue.Enqueue(coord1);
            queue.Enqueue(coord2);

            mockRedis.Setup(r => r.SaveCoordinateAsync(It.IsAny<GpsCoordinate>())).Returns(Task.CompletedTask);

            await service.ProcessPendingQueueAsync();

            mockRedis.Verify(r => r.SaveCoordinateAsync(coord1), Times.Once);
            mockRedis.Verify(r => r.SaveCoordinateAsync(coord2), Times.Once);
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public async Task ProcessPendingQueueAsync_ReEnqueues_WhenRedisFails()
        {
            ClearPendingQueue();
            var mockRedis = new Mock<IRedisClient>();
            var mockVehicleService = new Mock<IVehicleService>();
            var service = new GeolocationService(mockRedis.Object, mockVehicleService.Object);

            var queueField = typeof(GeolocationService)
                .GetField("_pendingQueue", BindingFlags.Static | BindingFlags.NonPublic);

            var queue = (System.Collections.Concurrent.ConcurrentQueue<GpsCoordinate>)queueField!.GetValue(null)!;

            var coord1 = new GpsCoordinate { VehicleId = "V1", Latitude = 1, Longitude = 2, Timestamp = DateTime.UtcNow };
            queue.Enqueue(coord1);

            mockRedis.Setup(r => r.SaveCoordinateAsync(coord1)).ThrowsAsync(new Exception("Redis error"));

            await service.ProcessPendingQueueAsync();

            mockRedis.Verify(r => r.SaveCoordinateAsync(coord1), Times.Once);
            Assert.False(queue.IsEmpty);
        }
    }
}