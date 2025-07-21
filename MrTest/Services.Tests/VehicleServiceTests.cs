using Cross.Dtos;
using Cross.Helpers.Context;
using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;
using Interfaces.Infrastructure.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Services.Implementations;
using System;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace MrTest.Services.Tests
{
    public partial class VehicleServiceTests
    {
        [Fact]
        public void GetVehicles_ReturnsMappedVehicleDtos()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new() { Id = "V1", Name = "Camión 1", LicensePlate = "ABC-123", Model = "Sprinter", Year = 2022, Status = "active", LastLocation = "Bogotá", LastSeen = DateTime.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture) },
                new() { Id = "V2", Name = "Camión 2", LicensePlate = "DEF-456", Model = "Iveco", Year = 2021, Status = "inactive", LastLocation = "Medellín", LastSeen = DateTime.Parse("2024-01-02T11:00:00Z", CultureInfo.InvariantCulture) }
            };
            var repoMock = new Mock<IVehicleRepository>();
            repoMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), It.IsAny<FindOption>()))
                .Returns(vehicles);

            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.GetVehicles();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("V1", result[0].Id);
            Assert.Equal("Camión 2", result[1].Name);
        }

        [Fact]
        public void GetVehicleById_ReturnsMappedVehicleDto_WhenExists()
        {
            // Arrange
            var vehicle = new Vehicle { Id = "V1", Name = "Camión 1" };
            var repoMock = new Mock<IVehicleRepository>();
            repoMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), It.IsAny<FindOption>()))
                .Returns([vehicle]);

            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.GetVehicleById("V1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("V1", result!.Id);
            Assert.Equal("Camión 1", result.Name);
        }

        [Fact]
        public void GetVehicleById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var repoMock = new Mock<IVehicleRepository>();
            repoMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), It.IsAny<FindOption>()))
                .Returns([]);

            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.GetVehicleById("NOEXISTE");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateVehicle_ReturnsTrue_WhenVehicleExists()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new() { Id = "V1", Name = "Camión 1", LicensePlate = "ABC-123", Model = "Sprinter", Year = 2022, Status = "active", LastLocation = "Bogotá", LastSeen = DateTime.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture) }
            };

            var vehicleDto = new VehicleDto
            {
                Id = "V1",
                Name = "Nuevo Nombre",
                LicensePlate = "NEW-123",
                Model = "Nuevo Modelo",
                Year = 2023,
                Status = "active",
                LastLocation = "Cali",
                LastSeen = DateTime.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture)
            };

            var repoMock = new Mock<IVehicleRepository>();
            repoMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), It.IsAny<FindOption>()))
                .Returns(vehicles);
            repoMock.Setup(r => r.Update(It.IsAny<Vehicle>()));

            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.UpdateVehicle(vehicleDto);

            // Assert
            Assert.True(result);
            Assert.Equal("Nuevo Nombre", vehicles[0].Name);
            Assert.Equal("NEW-123", vehicles[0].LicensePlate);
            Assert.Equal("Nuevo Modelo", vehicles[0].Model);
            Assert.Equal(2023, vehicles[0].Year);
            Assert.Equal("active", vehicles[0].Status);
            Assert.Equal("Cali", vehicles[0].LastLocation);
            Assert.Equal(DateTime.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture), vehicles[0].LastSeen);
            repoMock.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public void UpdateVehicle_ReturnsFalse_WhenVehicleDoesNotExist()
        {
            // Arrange
            var vehicles = new List<Vehicle>();

            var vehicleDto = new VehicleDto { Id = "NOEXISTE" };

            var repoMock = new Mock<IVehicleRepository>();
            repoMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), It.IsAny<FindOption>()))
                .Returns(vehicles);

            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.UpdateVehicle(vehicleDto);

            // Assert
            Assert.False(result);
            repoMock.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public void DeleteVehicleDistributed_CommitsTransactionAndReturnsTrue_WhenAllDeletesSucceed()
        {
            // Arrange
            var vehicle = new Vehicle { Id = "V1" };
            var repoMock = new Mock<IVehicleRepository>();
            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), null)).Returns([vehicle]);
            repoMock.Setup(r => r.Delete(vehicle));
            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            dbContextMock.Setup(x => x.SaveChanges()).Returns(1);
            redisMock.Setup(x => x.DeleteVehicleAsync("V1")).ReturnsAsync(true);
            transactionMock.Setup(t => t.Commit());

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act
            var result = service.DeleteVehicleDistributed("V1");

            // Assert
            Assert.True(result);
            repoMock.Verify(r => r.Delete(vehicle), Times.Once);
            redisMock.Verify(r => r.DeleteVehicleAsync("V1"), Times.Once);
            transactionMock.Verify(t => t.Commit(), Times.Once);
            transactionMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public void DeleteVehicleDistributed_Rollback_WhenRedisFails()
        {
            // Arrange
            var vehicle = new Vehicle { Id = "V1" };
            var repoMock = new Mock<IVehicleRepository>();
            var redisMock = new Mock<IRedisClient>();

            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            // Configura la propiedad Database para devolver el mock de DatabaseFacade
            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), null)).Returns([vehicle]);
            repoMock.Setup(r => r.Delete(vehicle));
            dbContextMock.Setup(x => x.SaveChanges()).Returns(1);
            redisMock.Setup(x => x.DeleteVehicleAsync("V1")).ReturnsAsync(false);
            transactionMock.Setup(t => t.Rollback());

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteVehicleDistributed("V1"));
            Assert.Contains("Rollback realizado por fallo en la transacción distribuida.", ex.Message);
            transactionMock.Verify(t => t.Rollback(), Times.Once);
        }

        [Fact]
        public void DeleteVehicleDistributed_Rollback_WhenExceptionThrown()
        {
            // Arrange
            var vehicle = new Vehicle { Id = "V1" };
            var repoMock = new Mock<IVehicleRepository>();
            var redisMock = new Mock<IRedisClient>();
            var dbContextMock = new Mock<PostgreSqlContext>(new DbContextOptions<PostgreSqlContext>());
            var dbDatabaseMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Vehicle, bool>>>(), null)).Returns([vehicle]);
            repoMock.Setup(r => r.Delete(vehicle)).Throws(new Exception("DB error"));
            dbContextMock.SetupGet(x => x.Database).Returns(dbDatabaseMock.Object);
            dbDatabaseMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            transactionMock.Setup(t => t.Rollback());

            var service = new VehicleService(repoMock.Object, redisMock.Object, dbContextMock.Object);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteVehicleDistributed("V1"));
            Assert.Contains("Rollback realizado por fallo en la transacción distribuida.", ex.Message);
            transactionMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}