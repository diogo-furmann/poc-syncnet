using System.Text.Json.Serialization;

namespace SyncNet.Api.DTOs.Sync;

public class CommentDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;

    [JsonPropertyName("task_id")]
    public string TaskId { get; set; } = null!;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("last_modified")]
    public long UpdatedAt { get; set; }

    [JsonPropertyName("_status")]
    public string? Status { get; set; }

    [JsonPropertyName("_changed")]
    public string? Changed { get; set; }
}
