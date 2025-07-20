using Cross.Dtos;
using Interfaces.Services;

namespace Services.Implementations
{
    public class CircuitBreakerService : ICircuitBreakerService
    {
        private int _consecutiveFailures = 0;
        private bool _circuitOpen = false;
        private readonly int _failureThreshold;

        public CircuitBreakerService(int failureThreshold = 3)
        {
            _failureThreshold = failureThreshold;
        }

        public bool IsOpen => _circuitOpen;
        public int ConsecutiveFailures => _consecutiveFailures;

        public void RegisterFailure()
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= _failureThreshold)
            {
                _circuitOpen = true;
            }
        }

        public void Reset()
        {
            _consecutiveFailures = 0;
            _circuitOpen = false;
        }

        public CircuitBreakerStatusDto GetStatus()
        {
            return new CircuitBreakerStatusDto
            {
                IsOpen = _circuitOpen,
                ConsecutiveFailures = _consecutiveFailures,
                FailureThreshold = _failureThreshold
            };
        }
    }
}
