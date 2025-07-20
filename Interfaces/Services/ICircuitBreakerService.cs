using Cross.Dtos;

namespace Interfaces.Services
{
    public interface ICircuitBreakerService
    {
        bool IsOpen { get; }
        int ConsecutiveFailures { get; }
        void RegisterFailure();
        void Reset();
        CircuitBreakerStatusDto GetStatus();
    }
}