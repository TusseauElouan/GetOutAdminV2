using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class ChannelMember
{
    public long Id { get; set; }

    public long ChannelId { get; set; }

    public long UserId { get; set; }

    public DateTime? LastReadAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
