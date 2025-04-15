using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class SanctionsUser
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long TypeReportUsersId { get; set; }

    public string? Description { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsPermanent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TypeReportUser TypeReportUsers { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
