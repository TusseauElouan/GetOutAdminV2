using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class Channel
{
    public long Id { get; set; }

    public long ServerId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string Type { get; set; } = null!;

    public bool IsPrivate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ChannelMember> ChannelMembers { get; set; } = new List<ChannelMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Server Server { get; set; } = null!;
}
