using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workspaces",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    last_modified = table.Column<long>(type: "INTEGER", nullable: false),
                    is_deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspaces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    workspace_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    last_modified = table.Column<long>(type: "INTEGER", nullable: false),
                    is_deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_projects_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    priority = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    project_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    last_modified = table.Column<long>(type: "INTEGER", nullable: false),
                    is_deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    task_id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    last_modified = table.Column<long>(type: "INTEGER", nullable: false),
                    is_deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_comments_id_updated_at",
                table: "comments",
                columns: new[] { "id", "last_modified" });

            migrationBuilder.CreateIndex(
                name: "idx_comments_task_id",
                table: "comments",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "idx_comments_updated_at",
                table: "comments",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "idx_projects_id_updated_at",
                table: "projects",
                columns: new[] { "id", "last_modified" });

            migrationBuilder.CreateIndex(
                name: "idx_projects_updated_at",
                table: "projects",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "idx_projects_workspace_id",
                table: "projects",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "idx_tasks_id_updated_at",
                table: "tasks",
                columns: new[] { "id", "last_modified" });

            migrationBuilder.CreateIndex(
                name: "idx_tasks_project_id",
                table: "tasks",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "idx_tasks_updated_at",
                table: "tasks",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "idx_workspaces_id_updated_at",
                table: "workspaces",
                columns: new[] { "id", "last_modified" });

            migrationBuilder.CreateIndex(
                name: "idx_workspaces_updated_at",
                table: "workspaces",
                column: "last_modified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "workspaces");
        }
    }
}
