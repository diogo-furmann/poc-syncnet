using SyncNet.Api.DTOs.Sync;

namespace SyncNet.Api.Services;

/// <summary>
/// Service interface for handling WatermelonDB synchronization protocol.
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Pull changes from the server since the last sync.
    /// </summary>
    /// <param name="lastPulledAt">Unix timestamp in milliseconds of the last pull. Use 0 for initial sync.</param>
    /// <param name="schemaVersion">Client schema version for compatibility checks.</param>
    /// <returns>All changes since lastPulledAt</returns>
    Task<SyncPullResponse> PullChangesAsync(long lastPulledAt, int schemaVersion);

    /// <summary>
    /// Push client changes to the server.
    /// </summary>
    /// <param name="request">Contains all client-side changes to persist.</param>
    /// <returns>Task representing the async operation.</returns>
    Task PushChangesAsync(SyncPushRequest request);
}
