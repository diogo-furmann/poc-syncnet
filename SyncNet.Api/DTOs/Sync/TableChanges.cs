namespace SyncNet.Api.DTOs.Sync;

/// <summary>
/// Represents changes for a specific table (created, updated, deleted records).
/// </summary>
/// <typeparam name="T">The DTO type for the entity</typeparam>
public class TableChanges<T>
{
    public List<T> Created { get; set; } = new();
    public List<T> Updated { get; set; } = new();
    public List<string> Deleted { get; set; } = new(); // Only IDs for deleted records
}
