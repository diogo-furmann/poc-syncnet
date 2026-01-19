using Microsoft.EntityFrameworkCore;
using SyncNet.Api.Data;
using SyncNet.Api.DTOs.Sync;
using SyncNet.Api.Entities;

namespace SyncNet.Api.Services;

/// <summary>
/// Implementation of the WatermelonDB synchronization protocol.
/// </summary>
public class SyncService : ISyncService
{
    private readonly SyncDbContext _context;
    private readonly ILogger<SyncService> _logger;

    public SyncService(SyncDbContext context, ILogger<SyncService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Pull changes from the server. Implements Clock Drift Protection by capturing timestamp before queries.
    /// </summary>
    public async Task<SyncPullResponse> PullChangesAsync(long lastPulledAt, int schemaVersion)
    {
        // Clock Drift Protection: Capture timestamp BEFORE executing queries
        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        _logger.LogInformation("Pull requested. LastPulledAt: {LastPulledAt}, SchemaVersion: {SchemaVersion}", 
            lastPulledAt, schemaVersion);

        var response = new SyncPullResponse
        {
            Timestamp = currentTimestamp,
            Changes = new PullChanges
            {
                Workspaces = await GetTableChangesAsync<Workspace, WorkspaceDto>(lastPulledAt, MapWorkspaceToDto),
                Projects = await GetTableChangesAsync<Project, ProjectDto>(lastPulledAt, MapProjectToDto),
                Tasks = await GetTableChangesAsync<Entities.Task, TaskDto>(lastPulledAt, MapTaskToDto),
                Comments = await GetTableChangesAsync<Comment, CommentDto>(lastPulledAt, MapCommentToDto)
            }
        };

        _logger.LogInformation("Pull completed. Total changes: W:{W} P:{P} T:{T} C:{C}", 
            response.Changes.Workspaces.Created.Count + response.Changes.Workspaces.Updated.Count + response.Changes.Workspaces.Deleted.Count,
            response.Changes.Projects.Created.Count + response.Changes.Projects.Updated.Count + response.Changes.Projects.Deleted.Count,
            response.Changes.Tasks.Created.Count + response.Changes.Tasks.Updated.Count + response.Changes.Tasks.Deleted.Count,
            response.Changes.Comments.Created.Count + response.Changes.Comments.Updated.Count + response.Changes.Comments.Deleted.Count);

        return response;
    }

    /// <summary>
    /// Generic method to fetch and categorize changes for any entity type.
    /// </summary>
    private async Task<TableChanges<TDto>> GetTableChangesAsync<TEntity, TDto>(
        long lastPulledAt, 
        Func<TEntity, TDto> mapper) 
        where TEntity : SyncableEntity
    {
        var changes = new TableChanges<TDto>();

        // Fetch all records modified since lastPulledAt (using AsNoTracking for performance)
        var modifiedRecords = await _context.Set<TEntity>()
            .AsNoTracking()
            .Where(e => e.UpdatedAt > lastPulledAt)
            .ToListAsync();

        foreach (var record in modifiedRecords)
        {
            if (record.IsDeleted)
            {
                // Record is deleted
                changes.Deleted.Add(record.Id);
            }
            else if (record.CreatedAt > lastPulledAt)
            {
                // Record was created after last pull
                changes.Created.Add(mapper(record));
            }
            else
            {
                // Record was updated (existed before last pull, but modified after)
                changes.Updated.Add(mapper(record));
            }
        }

        return changes;
    }

    /// <summary>
    /// Push client changes to server with full transaction support and referential integrity ordering.
    /// </summary>
    public async System.Threading.Tasks.Task PushChangesAsync(SyncPushRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Push started - processing client changes");

            // PHASE 1: Process INSERT and UPDATE in hierarchy order (parent → child)
            await ProcessUpsertAsync(request.Changes.Workspaces, MapDtoToWorkspace);
            await ProcessUpsertAsync(request.Changes.Projects, MapDtoToProject);
            await ProcessUpsertAsync(request.Changes.Tasks, MapDtoToTask);
            await ProcessUpsertAsync(request.Changes.Comments, MapDtoToComment);

            // PHASE 2: Process DELETE in reverse hierarchy order (child → parent)
            await ProcessDeleteAsync<Comment>(request.Changes.Comments.Deleted);
            await ProcessDeleteAsync<Entities.Task>(request.Changes.Tasks.Deleted);
            await ProcessDeleteAsync<Project>(request.Changes.Projects.Deleted);
            await ProcessDeleteAsync<Workspace>(request.Changes.Workspaces.Deleted);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Push completed successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Push failed - rolling back transaction");
            throw;
        }
    }

    /// <summary>
    /// Process upsert (insert or update) for a collection of DTOs.
    /// Server Authority: UpdatedAt is always set to server time, ignoring client values.
    /// </summary>
    private async System.Threading.Tasks.Task ProcessUpsertAsync<TEntity, TDto>(
        TableChanges<TDto> changes, 
        Func<TDto, TEntity> mapper) 
        where TEntity : SyncableEntity
    {
        var serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Process created records
        foreach (var dto in changes.Created)
        {
            var entity = mapper(dto);
            entity.UpdatedAt = serverTimestamp; // Server authority
            
            var existing = await _context.Set<TEntity>().FindAsync(entity.Id);
            if (existing == null)
            {
                _context.Set<TEntity>().Add(entity);
            }
            else
            {
                // Record already exists, update instead
                _context.Entry(existing).CurrentValues.SetValues(entity);
                existing.UpdatedAt = serverTimestamp;
            }
        }

        // Process updated records
        foreach (var dto in changes.Updated)
        {
            var entity = mapper(dto);
            entity.UpdatedAt = serverTimestamp; // Server authority

            var existing = await _context.Set<TEntity>().FindAsync(entity.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
                existing.UpdatedAt = serverTimestamp;
            }
            else
            {
                // Record doesn't exist, insert instead
                _context.Set<TEntity>().Add(entity);
            }
        }
    }

    /// <summary>
    /// Process soft delete for a collection of IDs.
    /// </summary>
    private async System.Threading.Tasks.Task ProcessDeleteAsync<TEntity>(List<string> deletedIds) 
        where TEntity : SyncableEntity
    {
        var serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        foreach (var id in deletedIds)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = serverTimestamp; // Server authority
            }
        }
    }

    #region Mapping Methods

    private static WorkspaceDto MapWorkspaceToDto(Workspace entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static ProjectDto MapProjectToDto(Project entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        WorkspaceId = entity.WorkspaceId,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static TaskDto MapTaskToDto(Entities.Task entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        Status = entity.Status,
        Priority = entity.Priority,
        ProjectId = entity.ProjectId,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static CommentDto MapCommentToDto(Comment entity) => new()
    {
        Id = entity.Id,
        Content = entity.Content,
        TaskId = entity.TaskId,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static Workspace MapDtoToWorkspace(WorkspaceDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt // Will be overridden by server timestamp
    };

    private static Project MapDtoToProject(ProjectDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        WorkspaceId = dto.WorkspaceId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };

    private static Entities.Task MapDtoToTask(TaskDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Description = dto.Description,
        Status = dto.Status,
        Priority = dto.Priority,
        ProjectId = dto.ProjectId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };

    private static Comment MapDtoToComment(CommentDto dto) => new()
    {
        Id = dto.Id,
        Content = dto.Content,
        TaskId = dto.TaskId,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };

    #endregion
}
