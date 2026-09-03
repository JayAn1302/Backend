namespace BarangayEquipmentAPI.DTOs
{
    public class EquipmentResponseDto
    {
        public int Id { get; set; }

        public string SerialNumber { get; set; } = string.Empty;

        public string EquipmentName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }

        public string Condition { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

    }
}