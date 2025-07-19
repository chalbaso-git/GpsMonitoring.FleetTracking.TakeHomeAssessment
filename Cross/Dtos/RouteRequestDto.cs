namespace Cross.Dtos
{
    public class RouteRequestDto
    {
        public required string VehicleId { get; set; }
        public required string Origin { get; set; }
        public required string Destination { get; set; }
    }
}