namespace BarangayEquipmentAPI.DTOs
{
    public class ForgotPasswordDto
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}