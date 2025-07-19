namespace Interfaces.Infrastructure.EF
{
    public interface IVehicleRepository
    {
        Task DeleteAsync(string vehicleId);
    }
}