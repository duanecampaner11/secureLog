using System;
using System.Collections.Generic;

namespace SecureLog.temp;

public partial class VisitRequest
{
    public int Id { get; set; }

    public string ClientUserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Purpose { get; set; } = null!;

    public DateTime VisitDate { get; set; }

    public string Company { get; set; } = null!;

    public string? Notes { get; set; }

    public int Status { get; set; }

    public string? ReturnReason { get; set; }

    public string? ConfirmationId { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public string? ReviewedByUserId { get; set; }

    public string PersonToMeet { get; set; } = null!;

    public DateTime VisitTime { get; set; }
}
