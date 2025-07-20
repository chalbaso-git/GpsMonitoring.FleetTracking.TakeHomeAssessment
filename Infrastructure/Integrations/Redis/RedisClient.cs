using Cross.Constants;
using Domain.Entities;
using Interfaces.Infrastructure.Redis;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Integrations.Redis
{
    public class RedisClient : IRedisClient
    {
        private readonly IDatabase _db;

        public RedisClient(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }

        public async Task<GpsCoordinate?> GetLastCoordinateAsync(string vehicleId)
        {
            var value = await _db.StringGetAsync(vehicleId);
            return value.HasValue ? JsonSerializer.Deserialize<GpsCoordinate>(value!) : null;
        }

        public async Task SaveCoordinateAsync(GpsCoordinate coordinate)
        {
            var json = JsonSerializer.Serialize(coordinate);
            await _db.StringSetAsync(coordinate.VehicleId, json, TimeSpan.FromMinutes(AppConstants.RedisTtlMinutes));
        }

        public async Task<bool> DeleteVehicleAsync(string vehicleId)
        {
            return await _db.KeyDeleteAsync(vehicleId);
        }
    }
}
