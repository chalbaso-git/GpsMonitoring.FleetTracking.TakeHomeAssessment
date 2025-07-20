using Services.Implementations;
using Interfaces.Infrastructure;
using Interfaces.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MrTest.Services.Tests
{
    public class VehicleServiceTests
    {
        [Fact]
        public async Task DeleteVehicleDistributedAsync_Rollback_WhenRedisFails()
        {
            var redisMock = new Mock<IRedisClient>();
            var repoMock = new Mock<IVehicleRepository>();
            var dbMock = new Mock<DbContext>();
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbMock.Object);

            dbMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransactionAsync(default)).ReturnsAsync(Mock.Of<IDbContextTransaction>());
            redisMock.Setup(x => x.DeleteVehicleAsync(It.IsAny<string>())).ReturnsAsync(false);

            var service = new VehicleService(redisMock.Object, repoMock.Object, dbMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteVehicleDistributedAsync("V1"));
        }

        [Fact]
        public async Task DeleteVehicleDistributedAsync_CommitsTransactionAndReturnsTrue_WhenAllDeletesSucceed()
        {
            // Arrange
            var mockRedis = new Mock<IRedisClient>();
            var mockRepo = new Mock<IVehicleRepository>();
            var mockDbContext = new Mock<DbContext>();
            var mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);

            var mockTransaction = new Mock<IDbContextTransaction>();
            mockDbContext.SetupGet(c => c.Database).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.BeginTransactionAsync(default))
                .ReturnsAsync(mockTransaction.Object);

            mockRepo.Setup(r => r.DeleteAsync("veh1")).Returns(Task.CompletedTask);
            mockRedis.Setup(r => r.DeleteVehicleAsync("veh1")).ReturnsAsync(true);

            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);

            var service = new VehicleService(mockRedis.Object, mockRepo.Object, mockDbContext.Object);

            // Act
            var result = await service.DeleteVehicleDistributedAsync("veh1");

            // Assert
            Assert.True(result);
            mockRepo.Verify(r => r.DeleteAsync("veh1"), Times.Once);
            mockRedis.Verify(r => r.DeleteVehicleAsync("veh1"), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(default), Times.Once);
            mockTransaction.Verify(t => t.RollbackAsync(default), Times.Never);
        }
    }

}