using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IGeolocationService
    {
        Task StoreCoordinateAsync(GpsCoordinateDto coordinateDto);
    }
}
