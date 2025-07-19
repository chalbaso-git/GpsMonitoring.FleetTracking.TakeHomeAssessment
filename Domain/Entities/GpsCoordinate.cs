namespace Domain.Entities
{
    public class GpsCoordinate
    {
        public required string VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
