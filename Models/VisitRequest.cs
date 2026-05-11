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
    
    public string? FullName { get; set; }
    public string? Company { get; set; }
    public string? Purpose { get; set; }
    public string? PersonToMeet { get; set; }
    
    [Required]
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    
    public string? Notes { get; set; }
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public string? ConfirmationId { get; set; }
    
    public string? ApprovedByUserId { get; set; }
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? ReturnReason { get; set; }
    
    // Provide default value for VisitTime (if database requires it)
    public DateTime VisitTime { get; set; } = DateTime.UtcNow;
}

public enum RequestStatus
{
    Pending,
    Confirmed,
    Returned,
    CheckedIn,
    Completed
}