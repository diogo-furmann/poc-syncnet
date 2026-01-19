using System.Text.Json.Serialization;

namespace SyncNet.Api.DTOs.Sync;

/// <summary>
/// Response for the Pull endpoint containing all changes since last sync.
/// </summary>
public class SyncPullResponse
{
    [JsonPropertyName("changes")]
    public PullChanges Changes { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}

public class PullChanges
{
    [JsonPropertyName("workspaces")]
    public TableChanges<WorkspaceDto> Workspaces { get; set; } = new();

    [JsonPropertyName("projects")]
    public TableChanges<ProjectDto> Projects { get; set; } = new();

    [JsonPropertyName("tasks")]
    public TableChanges<TaskDto> Tasks { get; set; } = new();

    [JsonPropertyName("comments")]
    public TableChanges<CommentDto> Comments { get; set; } = new();
}
