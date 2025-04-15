using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class TypeReportUser
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ReportUser> ReportUsers { get; set; } = new List<ReportUser>();

    public virtual ICollection<SanctionsUser> SanctionsUsers { get; set; } = new List<SanctionsUser>();
}
