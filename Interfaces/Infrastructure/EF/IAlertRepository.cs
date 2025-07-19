using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IAlertRepository
    {
        Task AddAsync(Alert alert);
        Task<List<Alert>> GetAllAsync();
    }
}