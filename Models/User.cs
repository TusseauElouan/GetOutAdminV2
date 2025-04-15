using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class User
{
    public long Id { get; set; }

    public string Prenom { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string Tag { get; set; } = null!;

    public string? ProfilePhotoUrl { get; set; }

    public string? Bio { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? EmailVerifiedAt { get; set; }

    public string Password { get; set; } = null!;

    public bool Private { get; set; }

    public DateTime? LastTagChange { get; set; }

    public bool IsAdmin { get; set; }

    public string? RememberToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChannelMember> ChannelMembers { get; set; } = new List<ChannelMember>();

    public virtual ICollection<Conversation> ConversationUser1s { get; set; } = new List<Conversation>();

    public virtual ICollection<Conversation> ConversationUser2s { get; set; } = new List<Conversation>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<PrivateMessage> PrivateMessages { get; set; } = new List<PrivateMessage>();

    public virtual ICollection<ReportUser> ReportUserReportedUsers { get; set; } = new List<ReportUser>();

    public virtual ICollection<ReportUser> ReportUserReporters { get; set; } = new List<ReportUser>();

    public virtual ICollection<ReportUser> ReportUserResolvedByNavigations { get; set; } = new List<ReportUser>();

    public virtual ICollection<ServerInvite> ServerInviteInvitees { get; set; } = new List<ServerInvite>();

    public virtual ICollection<ServerInvite> ServerInviteInviters { get; set; } = new List<ServerInvite>();

    public virtual ICollection<ServerMember> ServerMembers { get; set; } = new List<ServerMember>();

    public virtual ICollection<Server> Servers { get; set; } = new List<Server>();

    public virtual ICollection<UsersRelation> UsersRelationFriends { get; set; } = new List<UsersRelation>();

    public virtual ICollection<UsersRelation> UsersRelationUsers { get; set; } = new List<UsersRelation>();
}
