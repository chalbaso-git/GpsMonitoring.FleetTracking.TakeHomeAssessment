using System;
using System.Threading.Tasks;

namespace Mocks
{
    public class GpsVehicleSimulator
    {
        private readonly GpsCoordinateSender _sender;
        private readonly int _vehicleCount;
        private readonly int _intervalMs;
        private readonly double _failureRate;
        private readonly int _circuitBreakerThreshold;
        private int _consecutiveFailures = 0;
        private bool _circuitOpen = false;

        public GpsVehicleSimulator(
            string apiBaseUrl,
            int vehicleCount = 5,
            int intervalMs = 2000,
            double failureRate = 0.15,
            int circuitBreakerThreshold = 3)
        {
            _sender = new GpsCoordinateSender(apiBaseUrl);
            _vehicleCount = vehicleCount;
            _intervalMs = intervalMs;
            _failureRate = failureRate;
            _circuitBreakerThreshold = circuitBreakerThreshold;
        }

        public async Task RunAsync()
        {
            var random = new Random();
            while (true)
            {
                for (int i = 1; i <= _vehicleCount; i++)
                {
                    if (_circuitOpen)
                    {
                        Console.WriteLine("Circuit Breaker activado. No se envían coordenadas.");
                        continue;
                    }

                    if (random.NextDouble() < _failureRate)
                    {
                        Console.WriteLine($"[Vehículo {i}] Fallo simulado al enviar coordenada.");
                        IncrementFailure();
                        continue;
                    }

                    var coordinate = new
                    {
                        VehicleId = $"V{i}",
                        Latitude = 10.0 + random.NextDouble(),
                        Longitude = -74.0 + random.NextDouble(),
                        Timestamp = DateTime.UtcNow
                    };

                    try
                    {
                        var success = await _sender.SendCoordinateAsync(coordinate);
                        if (success)
                        {
                            Console.WriteLine($"[Vehículo {i}] Coordenada enviada correctamente.");
                            _consecutiveFailures = 0;
                        }
                        else
                        {
                            Console.WriteLine($"[Vehículo {i}] Error al enviar coordenada.");
                            IncrementFailure();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Vehículo {i}] Excepción: {ex.Message}");
                        IncrementFailure();
                    }
                }

                await Task.Delay(_intervalMs);
            }
        }

        private void IncrementFailure()
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= _circuitBreakerThreshold)
            {
                _circuitOpen = true;
                Console.WriteLine("Circuit Breaker activado por 3 fallos consecutivos.");
            }
        }

        public void ResetCircuit()
        {
            _circuitOpen = false;
            _consecutiveFailures = 0;
            Console.WriteLine("Circuit Breaker reiniciado.");
        }
    }
}