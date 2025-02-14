using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class TypeReportServer
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ReportServer> ReportServers { get; set; } = new List<ReportServer>();
}
