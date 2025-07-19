namespace Cross.Dtos
{
    public class AlertDto
    {
        public int Id { get; set; }
        public string VehicleId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}