using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureLog.Models;

public class VisitRequest
{
    [Key]
    public int Id { get; set; }
    
    public string? ClientUserId { get; set; }  // Changed from int? to string?
    
    [ForeignKey("ClientUserId")]
    public virtual ApplicationUser? ClientUser { get; set; }
    
    public string? FullName { get; set; }
    public string? Company { get; set; }
    public string? Purpose { get; set; }
    public string? PersonToMeet { get; set; }
    public DateTime VisitDate { get; set; }
    public DateTime? VisitTime { get; set; }
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
}

public enum RequestStatus
{
    Pending,
    Confirmed,
    Returned,
    CheckedIn,
    Completed
}