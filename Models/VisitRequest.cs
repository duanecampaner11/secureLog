using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SecureLog.Models;

public class VisitRequest
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string ClientUserId { get; set; } = string.Empty;
    public virtual ApplicationUser? ClientUser { get; set; }
    
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Purpose of Visit")]
    public string Purpose { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Visit Date")]
    public DateTime VisitDate { get; set; }
    
    [Display(Name = "Company/Organization")]
    public string? Company { get; set; }
    
    [Display(Name = "Person to Visit")]
    public string? PersonToVisit { get; set; }
    
    [Display(Name = "Status")]
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    
    [Display(Name = "Queue Number")]
    public string? QueueNumber { get; set; }
    
    [Display(Name = "Approved By")]
    public string? ApprovedByUserId { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    
    [Display(Name = "Rejection Reason")]
    public string? RejectionReason { get; set; }
}

public enum RequestStatus
{
    Pending,
    Approved,
    Denied,
    CheckedIn,
    Completed
}