namespace SyncNet.Api.Entities;

/// <summary>
/// Represents a task within a project.
/// </summary>
public class Task : SyncableEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = "pending"; // pending, in_progress, completed
    public string Priority { get; set; } = "medium"; // low, medium, high
    public string ProjectId { get; set; } = null!;

    // Navigation properties
    public Project Project { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
