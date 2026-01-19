using System.Text.Json.Serialization;

namespace SyncNet.Api.DTOs.Sync;

public class WorkspaceDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("last_modified")]
    public long UpdatedAt { get; set; }

    [JsonPropertyName("_status")]
    public string? Status { get; set; } // For WatermelonDB compatibility

    [JsonPropertyName("_changed")]
    public string? Changed { get; set; } // For WatermelonDB compatibility
}
