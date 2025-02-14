using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GetOutAdminV2.Models;

public partial class AppDbContext : DbContext
{

    private readonly IConfiguration _configuration;
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Cache> Caches { get; set; }

    public virtual DbSet<CacheLock> CacheLocks { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<ChannelMember> ChannelMembers { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<FailedJob> FailedJobs { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobBatch> JobBatches { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Migration> Migrations { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PrivateMessage> PrivateMessages { get; set; }

    public virtual DbSet<ReportServer> ReportServers { get; set; }

    public virtual DbSet<ReportUser> ReportUsers { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<ServerInvite> ServerInvites { get; set; }

    public virtual DbSet<ServerMember> ServerMembers { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<TypeReportServer> TypeReportServers { get; set; }

    public virtual DbSet<TypeReportUser> TypeReportUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersRelation> UsersRelations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cache>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("cache_pkey");

            entity.ToTable("cache");

            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Expiration).HasColumnName("expiration");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<CacheLock>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("cache_locks_pkey");

            entity.ToTable("cache_locks");

            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Expiration).HasColumnName("expiration");
            entity.Property(e => e.Owner)
                .HasMaxLength(255)
                .HasColumnName("owner");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("channels_pkey");

            entity.ToTable("channels");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPrivate)
                .HasDefaultValue(false)
                .HasColumnName("is_private");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValueSql("'text'::character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Server).WithMany(p => p.Channels)
                .HasForeignKey(d => d.ServerId)
                .HasConstraintName("channels_server_id_foreign");
        });

        modelBuilder.Entity<ChannelMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("channel_members_pkey");

            entity.ToTable("channel_members");

            entity.HasIndex(e => new { e.ChannelId, e.UserId }, "channel_members_channel_id_user_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.LastReadAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("last_read_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.ChannelMembers)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_members_channel_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.ChannelMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("channel_members_user_id_foreign");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversations_pkey");

            entity.ToTable("conversations");

            entity.HasIndex(e => new { e.User1Id, e.User2Id }, "conversations_user1_id_user2_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.LastMessageAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("last_message_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");

            entity.HasOne(d => d.User1).WithMany(p => p.ConversationUser1s)
                .HasForeignKey(d => d.User1Id)
                .HasConstraintName("conversations_user1_id_foreign");

            entity.HasOne(d => d.User2).WithMany(p => p.ConversationUser2s)
                .HasForeignKey(d => d.User2Id)
                .HasConstraintName("conversations_user2_id_foreign");
        });

        modelBuilder.Entity<FailedJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("failed_jobs_pkey");

            entity.ToTable("failed_jobs");

            entity.HasIndex(e => e.Uuid, "failed_jobs_uuid_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Connection).HasColumnName("connection");
            entity.Property(e => e.Exception).HasColumnName("exception");
            entity.Property(e => e.FailedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("failed_at");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.Queue).HasColumnName("queue");
            entity.Property(e => e.Uuid)
                .HasMaxLength(255)
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobs_pkey");

            entity.ToTable("jobs");

            entity.HasIndex(e => e.Queue, "jobs_queue_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attempts).HasColumnName("attempts");
            entity.Property(e => e.AvailableAt).HasColumnName("available_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.Queue)
                .HasMaxLength(255)
                .HasColumnName("queue");
            entity.Property(e => e.ReservedAt).HasColumnName("reserved_at");
        });

        modelBuilder.Entity<JobBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_batches_pkey");

            entity.ToTable("job_batches");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .HasColumnName("id");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FailedJobIds).HasColumnName("failed_job_ids");
            entity.Property(e => e.FailedJobs).HasColumnName("failed_jobs");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Options).HasColumnName("options");
            entity.Property(e => e.PendingJobs).HasColumnName("pending_jobs");
            entity.Property(e => e.TotalJobs).HasColumnName("total_jobs");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.HasIndex(e => new { e.ChannelId, e.CreatedAt }, "messages_channel_id_created_at_index");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "messages_user_id_created_at_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.EditedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("edited_at");
            entity.Property(e => e.FileMetadata)
                .HasColumnType("json")
                .HasColumnName("file_metadata");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.FileType)
                .HasMaxLength(255)
                .HasColumnName("file_type");
            entity.Property(e => e.Metadata)
                .HasColumnType("json")
                .HasColumnName("metadata");
            entity.Property(e => e.MimeType)
                .HasMaxLength(255)
                .HasColumnName("mime_type");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValueSql("'text'::character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("messages_channel_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("messages_user_id_foreign");
        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("migrations_pkey");

            entity.ToTable("migrations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Batch).HasColumnName("batch");
            entity.Property(e => e.Migration1)
                .HasMaxLength(255)
                .HasColumnName("migration");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("password_reset_tokens_pkey");

            entity.ToTable("password_reset_tokens");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
        });

        modelBuilder.Entity<PrivateMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("private_messages_pkey");

            entity.ToTable("private_messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DeletedByReceiverAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_by_receiver_at");
            entity.Property(e => e.DeletedBySenderAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_by_sender_at");
            entity.Property(e => e.EditedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("edited_at");
            entity.Property(e => e.Metadata)
                .HasColumnType("json")
                .HasColumnName("metadata");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("read_at");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValueSql("'text'::character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.PrivateMessages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("private_messages_conversation_id_foreign");

            entity.HasOne(d => d.Sender).WithMany(p => p.PrivateMessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("private_messages_sender_id_foreign");
        });

        modelBuilder.Entity<ReportServer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("report_servers_pkey");

            entity.ToTable("report_servers");

            entity.HasIndex(e => new { e.ReporterId, e.ServerId, e.TypeReportId }, "unique_server_report").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Evidence)
                .HasColumnType("json")
                .HasColumnName("evidence");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.ResolutionNote).HasColumnName("resolution_note");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TypeReportId).HasColumnName("type_report_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportServerReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("report_servers_reporter_id_foreign");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.ReportServerResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("report_servers_resolved_by_foreign");

            entity.HasOne(d => d.Server).WithMany(p => p.ReportServers)
                .HasForeignKey(d => d.ServerId)
                .HasConstraintName("report_servers_server_id_foreign");

            entity.HasOne(d => d.TypeReport).WithMany(p => p.ReportServers)
                .HasForeignKey(d => d.TypeReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_servers_type_report_id_foreign");
        });

        modelBuilder.Entity<ReportUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("report_users_pkey");

            entity.ToTable("report_users");

            entity.HasIndex(e => new { e.ReporterId, e.ReportedUserId, e.TypeReportId }, "unique_user_report").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Evidence)
                .HasColumnType("json")
                .HasColumnName("evidence");
            entity.Property(e => e.ReportedUserId).HasColumnName("reported_user_id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.ResolutionNote).HasColumnName("resolution_note");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TypeReportId).HasColumnName("type_report_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ReportUserReportedUsers)
                .HasForeignKey(d => d.ReportedUserId)
                .HasConstraintName("report_users_reported_user_id_foreign");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportUserReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("report_users_reporter_id_foreign");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.ReportUserResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("report_users_resolved_by_foreign");

            entity.HasOne(d => d.TypeReport).WithMany(p => p.ReportUsers)
                .HasForeignKey(d => d.TypeReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_users_type_report_id_foreign");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("servers_pkey");

            entity.ToTable("servers");

            entity.HasIndex(e => e.Slug, "servers_slug_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MaxMembers)
                .HasDefaultValue(100)
                .HasColumnName("max_members");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PrivacyType)
                .HasMaxLength(255)
                .HasDefaultValueSql("'private'::character varying")
                .HasColumnName("privacy_type");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Owner).WithMany(p => p.Servers)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("servers_owner_id_foreign");
        });

        modelBuilder.Entity<ServerInvite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("server_invites_pkey");

            entity.ToTable("server_invites");

            entity.HasIndex(e => e.Token, "server_invites_token_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("accepted_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.InviteeId).HasColumnName("invitee_id");
            entity.Property(e => e.InviterId).HasColumnName("inviter_id");
            entity.Property(e => e.RejectedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("rejected_at");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Invitee).WithMany(p => p.ServerInviteInvitees)
                .HasForeignKey(d => d.InviteeId)
                .HasConstraintName("server_invites_invitee_id_foreign");

            entity.HasOne(d => d.Inviter).WithMany(p => p.ServerInviteInviters)
                .HasForeignKey(d => d.InviterId)
                .HasConstraintName("server_invites_inviter_id_foreign");

            entity.HasOne(d => d.Server).WithMany(p => p.ServerInvites)
                .HasForeignKey(d => d.ServerId)
                .HasConstraintName("server_invites_server_id_foreign");
        });

        modelBuilder.Entity<ServerMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("server_members_pkey");

            entity.ToTable("server_members");

            entity.HasIndex(e => new { e.ServerId, e.UserId }, "server_members_server_id_user_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.LastReadAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("last_read_at");
            entity.Property(e => e.PrivacyConsent)
                .HasDefaultValue(false)
                .HasColumnName("privacy_consent");
            entity.Property(e => e.PrivacyConsentDate)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("privacy_consent_date");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasDefaultValueSql("'member'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Server).WithMany(p => p.ServerMembers)
                .HasForeignKey(d => d.ServerId)
                .HasConstraintName("server_members_server_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.ServerMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("server_members_user_id_foreign");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessions_pkey");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.LastActivity, "sessions_last_activity_index");

            entity.HasIndex(e => e.UserId, "sessions_user_id_index");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .HasColumnName("id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastActivity).HasColumnName("last_activity");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TypeReportServer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_report_servers_pkey");

            entity.ToTable("type_report_servers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TypeReportUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("type_report_users_pkey");

            entity.ToTable("type_report_users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_unique").IsUnique();

            entity.HasIndex(e => e.Tag, "users_tag_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.LastTagChange)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("last_tag_change");
            entity.Property(e => e.Nom)
                .HasMaxLength(255)
                .HasColumnName("nom");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Prenom)
                .HasMaxLength(255)
                .HasColumnName("prenom");
            entity.Property(e => e.Private)
                .HasDefaultValue(false)
                .HasColumnName("private");
            entity.Property(e => e.ProfilePhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("profile_photo_url");
            entity.Property(e => e.RememberToken)
                .HasMaxLength(100)
                .HasColumnName("remember_token");
            entity.Property(e => e.Tag)
                .HasMaxLength(30)
                .HasColumnName("tag");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UsersRelation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_relations_pkey");

            entity.ToTable("users_relations");

            entity.HasIndex(e => new { e.UserId, e.FriendId }, "users_relations_user_id_friend_id_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.PrivacyConsent)
                .HasDefaultValue(false)
                .HasColumnName("privacy_consent");
            entity.Property(e => e.PrivacyConsentDate)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("privacy_consent_date");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("read_at");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Friend).WithMany(p => p.UsersRelationFriends)
                .HasForeignKey(d => d.FriendId)
                .HasConstraintName("users_relations_friend_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.UsersRelationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("users_relations_user_id_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
