using System.Text.Json.Serialization;

namespace SyncNet.Api.DTOs.Sync;

public class TaskDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "medium";

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; } = null!;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("last_modified")]
    public long UpdatedAt { get; set; }

    [JsonPropertyName("_status")]
    public string? _Status { get; set; }

    [JsonPropertyName("_changed")]
    public string? _Changed { get; set; }
}
