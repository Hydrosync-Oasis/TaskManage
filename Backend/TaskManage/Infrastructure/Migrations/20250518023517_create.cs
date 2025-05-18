using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    LogContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Logs_T_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "T_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Project_T_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "T_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_TaskNode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deadline = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TaskStatus = table.Column<int>(type: "int", nullable: true),
                    CreateUserId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TaskNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_TaskNode_T_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "T_Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_TaskNode_T_User_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "T_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_TaskNode_T_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "T_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Comment_T_TaskNode_TaskId",
                        column: x => x.TaskId,
                        principalTable: "T_TaskNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Comment_T_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "T_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskNodeTaskNode",
                columns: table => new
                {
                    DependentNodesId = table.Column<int>(type: "int", nullable: false),
                    TaskNodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNodeTaskNode", x => new { x.DependentNodesId, x.TaskNodeId });
                    table.ForeignKey(
                        name: "FK_TaskNodeTaskNode_T_TaskNode_DependentNodesId",
                        column: x => x.DependentNodesId,
                        principalTable: "T_TaskNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskNodeTaskNode_T_TaskNode_TaskNodeId",
                        column: x => x.TaskNodeId,
                        principalTable: "T_TaskNode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Comment_OwnerId",
                table: "T_Comment",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Comment_TaskId",
                table: "T_Comment",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Logs_OwnerId",
                table: "T_Logs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Project_OwnerId",
                table: "T_Project",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TaskNode_AssignedUserId",
                table: "T_TaskNode",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TaskNode_CreateUserId",
                table: "T_TaskNode",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TaskNode_ProjectId",
                table: "T_TaskNode",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNodeTaskNode_TaskNodeId",
                table: "TaskNodeTaskNode",
                column: "TaskNodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Comment");

            migrationBuilder.DropTable(
                name: "T_Logs");

            migrationBuilder.DropTable(
                name: "TaskNodeTaskNode");

            migrationBuilder.DropTable(
                name: "T_TaskNode");

            migrationBuilder.DropTable(
                name: "T_Project");

            migrationBuilder.DropTable(
                name: "T_User");
        }
    }
}
