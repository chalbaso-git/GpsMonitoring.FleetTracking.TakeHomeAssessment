namespace Domain.Entities
{
    public class Alert
    {
        public int Id { get; set; }
        public string VehicleId { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}