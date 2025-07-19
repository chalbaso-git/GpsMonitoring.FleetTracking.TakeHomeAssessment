namespace Cross.Dtos
{
    public class AuditLogDto
    {
        public required string VehicleId { get; set; }
        public required string EventType { get; set; }
        public required string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}