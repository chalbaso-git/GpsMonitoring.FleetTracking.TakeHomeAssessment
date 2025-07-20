namespace Mocks
{
    public class Program
    {
        public static async Task Main()
        {
            string apiBaseUrl = "http://localhost:5000";
            var simulator = new GpsVehicleSimulator(apiBaseUrl);

            await simulator.RunAsync();
            while (true)
            {
                var input = Console.ReadLine();
                if (input?.Trim().ToLower() == "reset")
                {
                    simulator.ResetCircuit();
                }
            }
        }
    }
}
