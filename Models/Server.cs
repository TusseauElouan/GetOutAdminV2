using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class Server
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public long OwnerId { get; set; }

    public string PrivacyType { get; set; } = null!;

    public int MaxMembers { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<ReportServer> ReportServers { get; set; } = new List<ReportServer>();

    public virtual ICollection<ServerInvite> ServerInvites { get; set; } = new List<ServerInvite>();

    public virtual ICollection<ServerMember> ServerMembers { get; set; } = new List<ServerMember>();
}
