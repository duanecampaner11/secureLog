using Microsoft.AspNetCore.Identity;

namespace SecureLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; } = true;
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        
        // Navigation property - this is what the error is about
        public virtual ICollection<VisitRequest>? VisitRequests { get; set; }
    }
}