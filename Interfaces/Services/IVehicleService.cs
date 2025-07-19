using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IVehicleService
    {
        Task<bool> DeleteVehicleDistributedAsync(string vehicleId);
    }
}