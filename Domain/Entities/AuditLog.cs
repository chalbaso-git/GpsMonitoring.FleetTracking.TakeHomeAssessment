namespace Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EventType { get; set; } = default!;
        public string VehicleId { get; set; } = default!;
        public string Details { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}