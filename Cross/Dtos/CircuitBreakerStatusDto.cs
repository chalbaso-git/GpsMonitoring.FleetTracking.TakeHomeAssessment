namespace Cross.Dtos
{
    public class CircuitBreakerStatusDto
    {
        public bool IsOpen { get; set; }
        public int ConsecutiveFailures { get; set; }
        public int FailureThreshold { get; set; }
    }
}