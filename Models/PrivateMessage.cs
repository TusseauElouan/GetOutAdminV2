using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class PrivateMessage
{
    public long Id { get; set; }

    public long ConversationId { get; set; }

    public long SenderId { get; set; }

    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Metadata { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime? EditedAt { get; set; }

    public DateTime? DeletedBySenderAt { get; set; }

    public DateTime? DeletedByReceiverAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
