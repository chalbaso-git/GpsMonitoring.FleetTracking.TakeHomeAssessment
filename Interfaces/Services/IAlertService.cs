using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IAlertService
    {
        void AddAlert(AlertDto dto);
        List<AlertDto> GetAlerts();
    }
}