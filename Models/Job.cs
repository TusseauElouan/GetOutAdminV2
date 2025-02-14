using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class Job
{
    public long Id { get; set; }

    public string Queue { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public short Attempts { get; set; }

    public int? ReservedAt { get; set; }

    public int AvailableAt { get; set; }

    public int CreatedAt { get; set; }
}
