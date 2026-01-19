using Microsoft.EntityFrameworkCore;
using SyncNet.Api.Entities;

namespace SyncNet.Api.Data;

/// <summary>
/// Database context for the SyncNet application.
/// Manages all syncable entities with optimizations for the WatermelonDB protocol.
/// </summary>
public class SyncDbContext : DbContext
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Entities.Task> Tasks => Set<Entities.Task>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Workspace
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.ToTable("workspaces");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("last_modified");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            // Composite index for sync queries (Id, UpdatedAt)
            entity.HasIndex(e => new { e.Id, e.UpdatedAt }).HasDatabaseName("idx_workspaces_id_updated_at");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("idx_workspaces_updated_at");
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id").HasMaxLength(36).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("last_modified");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            // Foreign key relationship
            entity.HasOne(e => e.Workspace)
                .WithMany(w => w.Projects)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite index for sync queries
            entity.HasIndex(e => new { e.Id, e.UpdatedAt }).HasDatabaseName("idx_projects_id_updated_at");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("idx_projects_updated_at");
            entity.HasIndex(e => e.WorkspaceId).HasDatabaseName("idx_projects_workspace_id");
        });

        // Configure Task
        modelBuilder.Entity<Entities.Task>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(e => e.Priority).HasColumnName("priority").HasMaxLength(50);
            entity.Property(e => e.ProjectId).HasColumnName("project_id").HasMaxLength(36).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("last_modified");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            // Foreign key relationship
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite index for sync queries
            entity.HasIndex(e => new { e.Id, e.UpdatedAt }).HasDatabaseName("idx_tasks_id_updated_at");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("idx_tasks_updated_at");
            entity.HasIndex(e => e.ProjectId).HasDatabaseName("idx_tasks_project_id");
        });

        // Configure Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.TaskId).HasColumnName("task_id").HasMaxLength(36).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("last_modified");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            // Foreign key relationship
            entity.HasOne(e => e.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite index for sync queries
            entity.HasIndex(e => new { e.Id, e.UpdatedAt }).HasDatabaseName("idx_comments_id_updated_at");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("idx_comments_updated_at");
            entity.HasIndex(e => e.TaskId).HasDatabaseName("idx_comments_task_id");
        });
    }
}
