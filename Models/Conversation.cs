using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class Conversation
{
    public long Id { get; set; }

    public long User1Id { get; set; }

    public long User2Id { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<PrivateMessage> PrivateMessages { get; set; } = new List<PrivateMessage>();

    public virtual User User1 { get; set; } = null!;

    public virtual User User2 { get; set; } = null!;
}
