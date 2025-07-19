namespace Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public required string VehicleId { get; set; }
        public required string Action { get; set; }
        public required string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}