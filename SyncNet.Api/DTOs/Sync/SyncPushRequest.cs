using System.Text.Json.Serialization;

namespace SyncNet.Api.DTOs.Sync;

/// <summary>
/// Request for the Push endpoint containing all client changes.
/// </summary>
public class SyncPushRequest
{
    [JsonPropertyName("changes")]
    public PushChanges Changes { get; set; } = new();
}

public class PushChanges
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
