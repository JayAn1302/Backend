namespace BarangayEquipmentAPI.DTOs
{
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Staff";
    }
}