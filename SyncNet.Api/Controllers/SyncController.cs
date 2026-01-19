using Microsoft.AspNetCore.Mvc;
using SyncNet.Api.DTOs.Sync;
using SyncNet.Api.Services;

namespace SyncNet.Api.Controllers;

/// <summary>
/// Controller for WatermelonDB synchronization endpoints.
/// </summary>
[ApiController]
[Route("sync")]
[Produces("application/json")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncController> _logger;

    public SyncController(ISyncService syncService, ILogger<SyncController> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    /// <summary>
    /// Pull endpoint - retrieves all changes since last sync.
    /// </summary>
    /// <param name="last_pulled_at">Unix timestamp in milliseconds. Use 0 for initial sync.</param>
    /// <param name="schema_version">Client schema version for compatibility checks.</param>
    /// <returns>Changes since last_pulled_at with current server timestamp.</returns>
    [HttpGet("pull")]
    [ProducesResponseType(typeof(SyncPullResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncPullResponse>> Pull(
        [FromQuery] long? last_pulled_at = 0,
        [FromQuery] int schema_version = 1)
    {
        _logger.LogInformation("Pull request received. last_pulled_at={LastPulledAt}, schema_version={SchemaVersion}", 
            last_pulled_at, schema_version);

        var response = await _syncService.PullChangesAsync(last_pulled_at ?? 0, schema_version);
        return Ok(response);
    }

    /// <summary>
    /// Push endpoint - persists client changes to the server.
    /// </summary>
    /// <param name="request">Client changes to persist.</param>
    /// <returns>200 OK on success.</returns>
    [HttpPost("push")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Push([FromBody] SyncPushRequest request)
    {
        if (request?.Changes == null)
        {
            return BadRequest(new { error = "Invalid request", message = "Changes cannot be null" });
        }

        _logger.LogInformation("Push request received");

        await _syncService.PushChangesAsync(request);

        return Ok(new { message = "Changes synchronized successfully" });
    }
}
