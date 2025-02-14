using System;
using System.Collections.Generic;

namespace GetOutAdminV2.Models;

public partial class Message
{
    public long Id { get; set; }

    public long ChannelId { get; set; }

    public long UserId { get; set; }

    public string? Content { get; set; }

    public string Type { get; set; } = null!;

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public string? MimeType { get; set; }

    public string? FileMetadata { get; set; }

    public string? Metadata { get; set; }

    public DateTime? EditedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
