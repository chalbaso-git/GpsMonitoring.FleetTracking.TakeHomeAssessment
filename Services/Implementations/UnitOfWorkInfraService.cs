using Interfaces.Infrastructure.EF;
using Interfaces.Services;

namespace Services.Implementations
{
    public class UnitOfWorkInfra : IUnitOfWorkInfraService
    {
        public IGpsCoordinateRepository Coordinates { get; }

        public UnitOfWorkInfra(IGpsCoordinateRepository coordinates)
        {
            Coordinates = coordinates;
        }
    }
}
