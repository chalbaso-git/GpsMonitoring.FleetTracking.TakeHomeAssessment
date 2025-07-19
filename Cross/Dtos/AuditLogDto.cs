namespace Cross.Dtos
{
    public class AuditLogDto
    {
        public required string VehicleId { get; set; }
        public required string Action { get; set; }
        public required string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}