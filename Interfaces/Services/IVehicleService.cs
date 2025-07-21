using Cross.Dtos;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IVehicleService
    {
        bool DeleteVehicleDistributed(string vehicleId);
        List<VehicleDto> GetVehicles();
        VehicleDto? GetVehicleById(string id);
        bool UpdateVehicle(VehicleDto vehicleDto);
    }
}