namespace SyncNet.Api.Entities;

/// <summary>
/// Represents a workspace that contains multiple projects.
/// </summary>
public class Workspace : SyncableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation property
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
