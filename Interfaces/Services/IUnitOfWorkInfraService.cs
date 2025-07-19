using Interfaces.Infrastructure.EF;

namespace Interfaces.Services
{
    public interface IUnitOfWorkInfraService
    {
        IGpsCoordinateRepository Coordinates { get; }
    }
}
