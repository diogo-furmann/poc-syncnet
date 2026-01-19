namespace SyncNet.Api.Entities;

/// <summary>
/// Represents a project within a workspace.
/// </summary>
public class Project : SyncableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string WorkspaceId { get; set; } = null!;

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
