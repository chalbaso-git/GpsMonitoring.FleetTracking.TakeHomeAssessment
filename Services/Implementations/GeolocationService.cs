using Cross.Constants;
using Cross.Dtos;
using Cross.Helpers;
using Domain.Entities;
using Interfaces.Infrastructure;
using Interfaces.Services;

namespace Services.Implementations
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IRedisClient _redisClient;

        public GeolocationService(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public async Task StoreCoordinateAsync(GpsCoordinateDto coordinateDto)
        {
            try
            {
                ValidateCoordinate(coordinateDto);

                var last = await _redisClient.GetLastCoordinateAsync(coordinateDto.VehicleId);
                var current = MapToEntity(coordinateDto);

                if (IsDuplicate(last, current))
                    return;

                await _redisClient.SaveCoordinateAsync(current);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al almacenar la coordenada GPS.", ex);
            }
        }

        private static void ValidateCoordinate(GpsCoordinateDto coordinateDto)
        {
            var spec = new GpsCoordinateSpecification();
            if (!spec.IsSatisfiedBy(coordinateDto))
                throw new ArgumentException("Invalid GPS coordinates.");
        }

        private static GpsCoordinate MapToEntity(GpsCoordinateDto dto) =>
            new()
            {
                VehicleId = dto.VehicleId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timestamp = dto.Timestamp
            };

        private static bool IsDuplicate(GpsCoordinate? last, GpsCoordinate current)
        {
            if (last == null) return false;

            var lastDto = MapToDto(last);
            var currentDto = MapToDto(current);
            var distance = GeoUtils.CalculateDistance(lastDto, currentDto);
            return distance < AppConstants.DuplicateThresholdMeters;
        }

        private static GpsCoordinateDto MapToDto(GpsCoordinate entity) =>
            new()
            {
                VehicleId = entity.VehicleId,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Timestamp = entity.Timestamp
            };
    }
}
