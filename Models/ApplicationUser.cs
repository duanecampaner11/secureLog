using Microsoft.AspNetCore.Identity;

namespace SecureLog.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; } = true;
    public string? CompanyName { get; set; }
    
    [PersonalData]
    public new string? PhoneNumber { get; set; }  // Added 'new' keyword
    
    // Navigation property for visit requests
    public virtual ICollection<VisitRequest>? VisitRequests { get; set; }
}