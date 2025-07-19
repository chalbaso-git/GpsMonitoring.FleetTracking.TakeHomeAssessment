namespace Cross.Dtos
{
    public class RouteResponseDto
    {
        public required string VehicleId { get; set; }
        public required List<string> Path { get; set; }
        public double Distance { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}