using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class ServerMember
{
    public long Id { get; set; }

    public long ServerId { get; set; }

    public long UserId { get; set; }

    public string Role { get; set; } = null!;

    public bool PrivacyConsent { get; set; }

    public DateTime? PrivacyConsentDate { get; set; }

    public DateTime? LastReadAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Server Server { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
