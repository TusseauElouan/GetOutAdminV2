using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class UsersRelation
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long FriendId { get; set; }

    public DateTime? ReadAt { get; set; }

    public string Status { get; set; } = null!;

    public bool PrivacyConsent { get; set; }

    public DateTime? PrivacyConsentDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User Friend { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
