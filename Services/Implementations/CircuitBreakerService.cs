using Cross.Dtos;
using Interfaces.Services;

namespace Services.Implementations
{
    public class CircuitBreakerService : ICircuitBreakerService
    {
        private int _consecutiveFailures = 0;
        private bool _circuitOpen = false;
        private readonly int _failureThreshold;

        /// <summary>
        /// Inicializa una nueva instancia del servicio Circuit Breaker.
        /// </summary>
        /// <param name="failureThreshold">Número de fallos consecutivos para abrir el circuito. Por defecto es 3.</param>
        public CircuitBreakerService(int failureThreshold = 3)
        {
            _failureThreshold = failureThreshold;
        }

        /// <summary>
        /// Indica si el circuito está abierto (servicio desactivado por fallos consecutivos).
        /// </summary>
        public bool IsOpen => _circuitOpen;

        /// <summary>
        /// Número de fallos consecutivos registrados.
        /// </summary>
        public int ConsecutiveFailures => _consecutiveFailures;

        /// <summary>
        /// Registra un fallo en el servicio. Si se supera el umbral, el circuito se abre.
        /// </summary>
        public void RegisterFailure()
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= _failureThreshold)
            {
                _circuitOpen = true;
            }
        }

        /// <summary>
        /// Reinicia el estado del Circuit Breaker, cerrando el circuito y restableciendo el contador de fallos.
        /// </summary>
        public void Reset()
        {
            _consecutiveFailures = 0;
            _circuitOpen = false;
        }

        /// <summary>
        /// Obtiene el estado actual del Circuit Breaker.
        /// </summary>
        /// <returns>Un objeto <see cref="CircuitBreakerStatusDto"/> con el estado, número de fallos y umbral configurado.</returns>
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
