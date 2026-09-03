namespace BarangayEquipmentAPI.DTOs
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string Role { get; set; } = "Staff";

        public bool IsActive { get; set; }
    }
}