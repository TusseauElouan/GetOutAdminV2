using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class ServerInvite
{
    public long Id { get; set; }

    public long ServerId { get; set; }

    public long InviterId { get; set; }

    public long InviteeId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User Invitee { get; set; } = null!;

    public virtual User Inviter { get; set; } = null!;

    public virtual Server Server { get; set; } = null!;
}
