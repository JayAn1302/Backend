using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Admin";

    public bool IsActive { get; set; } = true;

    // New: for password reset
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
}