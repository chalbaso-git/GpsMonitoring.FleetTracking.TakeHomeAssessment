namespace Cross.Dtos
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string VehicleId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public double Distance { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}