using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IAlertService
    {
        Task AddAlertAsync(AlertDto dto);
        Task<List<AlertDto>> GetAlertsAsync();
    }
}