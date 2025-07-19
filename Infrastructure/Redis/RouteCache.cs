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
            var key = $"{route.VehicleId}:{route.Path.First()}:{route.Path.Last()}";
            var json = JsonSerializer.Serialize(route);
            await _db.StringSetAsync(key, json, TimeSpan.FromMinutes(5));
        }
    }
}