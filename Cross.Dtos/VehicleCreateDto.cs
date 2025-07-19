namespace Cross.Dtos
{
    public class VehicleCreateDto
    {
        public required string Id { get; set; }
        public required string Plate { get; set; }
        public string? Model { get; set; }
        public string? Status { get; set; }
    }
}