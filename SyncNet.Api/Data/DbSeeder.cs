using Microsoft.EntityFrameworkCore;
using SyncNet.Api.Entities;

namespace SyncNet.Api.Data;

/// <summary>
/// Seeds the database with initial data for development/testing.
/// </summary>
public static class DbSeeder
{
    public static async System.Threading.Tasks.Task SeedAsync(SyncDbContext context, ILogger logger)
    {
        // Check if database already has data
        if (await context.Workspaces.AnyAsync())
        {
            logger.LogInformation("Database already contains data. Skipping seed.");
            return;
        }

        logger.LogInformation("Seeding database with initial data...");

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Create Workspaces
        var workspace1 = new Workspace
        {
            Id = "ws-demo-001",
            Name = "Demo Workspace",
            Description = "Workspace de demonstração",
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var workspace2 = new Workspace
        {
            Id = "ws-personal-001",
            Name = "Personal Projects",
            Description = "Projetos pessoais",
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Workspaces.AddRange(workspace1, workspace2);

        // Create Projects
        var project1 = new Project
        {
            Id = "proj-demo-001",
            Name = "API Development",
            Description = "Desenvolvimento da API de sincronização",
            WorkspaceId = workspace1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var project2 = new Project
        {
            Id = "proj-demo-002",
            Name = "Frontend App",
            Description = "Aplicação mobile com WatermelonDB",
            WorkspaceId = workspace1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var project3 = new Project
        {
            Id = "proj-personal-001",
            Name = "Learning Project",
            Description = "Estudos e experimentos",
            WorkspaceId = workspace2.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Projects.AddRange(project1, project2, project3);

        // Create Tasks
        var task1 = new Entities.Task
        {
            Id = "task-001",
            Title = "Implementar endpoint de Pull",
            Description = "Desenvolver lógica de extração de deltas",
            Status = "completed",
            Priority = "high",
            ProjectId = project1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var task2 = new Entities.Task
        {
            Id = "task-002",
            Title = "Implementar endpoint de Push",
            Description = "Desenvolver lógica de persistência com transações",
            Status = "completed",
            Priority = "high",
            ProjectId = project1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var task3 = new Entities.Task
        {
            Id = "task-003",
            Title = "Criar testes automatizados",
            Description = "Desenvolver testes com Hurl",
            Status = "in_progress",
            Priority = "medium",
            ProjectId = project1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var task4 = new Entities.Task
        {
            Id = "task-004",
            Title = "Integrar WatermelonDB no app",
            Description = "Configurar banco local no mobile",
            Status = "pending",
            Priority = "high",
            ProjectId = project2.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var task5 = new Entities.Task
        {
            Id = "task-005",
            Title = "Implementar sincronização offline",
            Description = "Garantir que app funcione sem conexão",
            Status = "pending",
            Priority = "medium",
            ProjectId = project2.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var task6 = new Entities.Task
        {
            Id = "task-006",
            Title = "Estudar .NET 8",
            Description = "Aprender novas features do .NET 8",
            Status = "in_progress",
            Priority = "low",
            ProjectId = project3.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Tasks.AddRange(task1, task2, task3, task4, task5, task6);

        // Create Comments
        var comment1 = new Comment
        {
            Id = "comment-001",
            Content = "Implementado com Clock Drift Protection ✅",
            TaskId = task1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var comment2 = new Comment
        {
            Id = "comment-002",
            Content = "Usando AsNoTracking() para performance",
            TaskId = task1.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var comment3 = new Comment
        {
            Id = "comment-003",
            Content = "Transações atômicas implementadas com sucesso",
            TaskId = task2.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var comment4 = new Comment
        {
            Id = "comment-004",
            Content = "Server Authority garantindo consistência dos timestamps",
            TaskId = task2.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var comment5 = new Comment
        {
            Id = "comment-005",
            Content = "Arquivos .hurl criados para pull, push e sync-flow",
            TaskId = task3.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var comment6 = new Comment
        {
            Id = "comment-006",
            Content = "Precisamos validar offline-first no app",
            TaskId = task4.Id,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Comments.AddRange(comment1, comment2, comment3, comment4, comment5, comment6);

        // Save all changes
        await context.SaveChangesAsync();

        logger.LogInformation("Database seeded successfully!");
        logger.LogInformation("  - Workspaces: 2");
        logger.LogInformation("  - Projects: 3");
        logger.LogInformation("  - Tasks: 6");
        logger.LogInformation("  - Comments: 6");
    }
}
