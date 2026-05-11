using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureLog.Models;

public class VisitRequest
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string ClientUserId { get; set; } = string.Empty;
    
    [ForeignKey("ClientUserId")]
    public virtual ApplicationUser? ClientUser { get; set; }
    
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Company/Organization")]
    public string? Company { get; set; }
    
    [Required]
    [Display(Name = "Purpose of Visit")]
    public string Purpose { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Person/Department to Meet")]
    public string PersonToMeet { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Visit Date")]
    public DateTime VisitDate { get; set; }
    
    [Required]
    [Display(Name = "Visit Time")]
    public DateTime VisitTime { get; set; }
    
    [Display(Name = "Additional Notes")]
    public string? Notes { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    
    [Display(Name = "Confirmation ID")]
    public string? ConfirmationId { get; set; }
    
    [Display(Name = "Approved/Denied By")]
    public string? ReviewedByUserId { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    
    [Display(Name = "Return Reason")]
    public string? ReturnReason { get; set; }
}

public enum RequestStatus
{
    Pending,    // Waiting for admin approval
    Confirmed,  // Admin approved - confirmation ID generated
    Returned,   // Admin denied - sent back to client
    CheckedIn,  // Guard verified at front desk
    Completed   // Visitor checked out
}