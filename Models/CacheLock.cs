using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class CacheLock
{
    public string Key { get; set; } = null!;

    public string Owner { get; set; } = null!;

    public int Expiration { get; set; }
}
