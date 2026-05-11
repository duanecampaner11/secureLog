using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureLog.Models;

public class VisitRequest
{
    [Key]
    public int Id { get; set; }
    
    public int? ClientUserId { get; set; }
    
    [ForeignKey("ClientUserId")]
    public virtual ApplicationUser? ClientUser { get; set; }
    
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    public string Purpose { get; set; } = string.Empty;
    
    [Required]
    public DateTime VisitDate { get; set; }
    
    public string Company { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    
    public string ReturnReason { get; set; } = string.Empty;
    
    public string ConfirmationId { get; set; } = string.Empty;
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ReviewedAt { get; set; }
    
    public DateTime CheckInTime { get; set; }
    
    public DateTime CheckOutTime { get; set; }
    
    public string ReviewedByUserId { get; set; } = string.Empty;
    
    public string VisitorId { get; set; } = string.Empty;
    
    public DateTime ApprovedAt { get; set; }
    
    public string ApprovedByUserId { get; set; } = string.Empty;
    
    // Optional - if you need VisitTime, use this:
    [NotMapped]
    public DateTime? VisitTime { get; set; }
    
    // Optional - if you need PersonToMeet:
    [NotMapped]
    public string? PersonToMeet { get; set; }
}

public enum RequestStatus
{
    Pending,
    Confirmed,
    Returned,
    CheckedIn,
    Completed
}