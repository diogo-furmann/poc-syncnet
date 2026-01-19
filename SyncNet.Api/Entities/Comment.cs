namespace SyncNet.Api.Entities;

/// <summary>
/// Represents a comment on a task.
/// </summary>
public class Comment : SyncableEntity
{
    public string Content { get; set; } = null!;
    public string TaskId { get; set; } = null!;

    // Navigation property
    public Task Task { get; set; } = null!;
}
