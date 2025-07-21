namespace Cross.Dtos
{
    public class VehicleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Status { get; set; } = "active";
        public string LastLocation { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
    }
}