namespace Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string VehicleId { get; set; } = default!;
        public string Path { get; set; } = default!; // Puedes guardar como JSON o texto
        public double Distance { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}