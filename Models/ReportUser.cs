using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class ReportUser
{
    public long Id { get; set; }

    public long ReporterId { get; set; }

    public long ReportedUserId { get; set; }

    public long TypeReportId { get; set; }

    public string? Description { get; set; }

    public string? Evidence { get; set; }

    public string Status { get; set; } = null!;

    public string? ResolutionNote { get; set; }

    public long? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User ReportedUser { get; set; } = null!;

    public virtual User Reporter { get; set; } = null!;

    public virtual User? ResolvedByNavigation { get; set; }

    public virtual TypeReportUser TypeReport { get; set; } = null!;
}
