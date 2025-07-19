using Domain.Entities;
using Interfaces.Infrastructure;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Redis
{
    public class RouteCache : IRouteCache
    {
        private readonly IDatabase _db;

        public RouteCache(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }

        public async Task<Route?> GetCachedRouteAsync(string vehicleId, string origin, string destination)
        {
            var key = $"{vehicleId}:{origin}:{destination}";
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<Route>(value!) : null;
        }

        public async Task SaveRouteAsync(Route route)
        {
            var key = $"{route.VehicleId}:{route.Path[0]}:{route.Path[^1]}";
            var json = JsonSerializer.Serialize(route);
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(5));
        }

        // Locking distribuido para zona
        public async Task<bool> AcquireZoneLockAsync(string origin, string destination, string vehicleId, TimeSpan timeout)
        {
            var lockKey = $"lock:zone:{origin}:{destination}";
            return await _db.StringSetAsync(lockKey, vehicleId, timeout, When.NotExists);
        }

        public async Task ReleaseZoneLockAsync(string origin, string destination, string vehicleId)
        {
            var lockKey = $"lock:zone:{origin}:{destination}";
            var value = await _db.StringGetAsync(lockKey);
            if (value == vehicleId)
                await _db.KeyDeleteAsync(lockKey);
        }
    }
}