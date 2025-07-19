namespace Cross.Dtos
{
    public class GpsCoordinateDto
    {
        public required string VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
