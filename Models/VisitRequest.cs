using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureLog.Models;

public class VisitRequest
{
    [Key]
    public int Id { get; set; }
    
    public string? ClientUserId { get; set; }
    
    [ForeignKey("ClientUserId")]
    public virtual ApplicationUser? ClientUser { get; set; }
    
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }
    
    [Display(Name = "Company")]
    public string? Company { get; set; }
    
    [Display(Name = "Purpose of Visit")]
    public string? Purpose { get; set; }
    
    [Display(Name = "Person to Meet")]
    public string? PersonToMeet { get; set; }
    
    [Display(Name = "Visit Date")]
    [DataType(DataType.Date)]
    public DateTime? VisitDate { get; set; }
    
    [Display(Name = "Visit Time")]
    [DataType(DataType.Time)]
    public DateTime? VisitTime { get; set; }
    
    [Display(Name = "Additional Notes")]
    public string? Notes { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    
    [Display(Name = "Confirmation ID")]
    public string? ConfirmationId { get; set; }
    
    public string? ApprovedByUserId { get; set; }
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    
    [Display(Name = "Return Reason")]
    public string? ReturnReason { get; set; }
}

public enum RequestStatus
{
    Pending,
    Confirmed,
    Returned,
    CheckedIn,
    Completed
}