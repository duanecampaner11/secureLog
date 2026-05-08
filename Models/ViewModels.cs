using System.ComponentModel.DataAnnotations;

namespace SecureLog.Models;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class AddGuestViewModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Purpose { get; set; } = string.Empty;
}

public class DashboardViewModel
{
    public AddGuestViewModel NewGuest { get; set; } = new();
    public List<GuestEntry> Entries { get; set; } = new();
    public string? Search { get; set; }
    public Dictionary<string, string> UserNames { get; set; } = new();
}
