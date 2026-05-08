using Microsoft.AspNetCore.Identity;

namespace SecureLog.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}