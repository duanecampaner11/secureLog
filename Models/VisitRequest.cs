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
    
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Display(Name = "Company")]
    public string? Company { get; set; }
    
    [Required(ErrorMessage = "Purpose of visit is required")]
    [Display(Name = "Purpose of Visit")]
    public string Purpose { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Person to meet is required")]
    [Display(Name = "Person to Meet")]
    public string PersonToMeet { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Visit date is required")]
    [Display(Name = "Visit Date")]
    [DataType(DataType.Date)]
    public DateTime VisitDate { get; set; }
    
    [Required(ErrorMessage = "Visit time is required")]
    [Display(Name = "Visit Time")]
    [DataType(DataType.Time)]
    public DateTime VisitTime { get; set; }
    
    [Display(Name = "Additional Notes")]
    public string? Notes { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    
    [Display(Name = "Confirmation ID")]
    public string? ConfirmationId { get; set; }
    
    public string? ApprovedByUserId { get; set; }
    
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