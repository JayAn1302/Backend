namespace BarangayEquipmentAPI.DTOs
{
    public class BorrowerResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        // NEW
        public string? PhotoPath { get; set; }
    }
}