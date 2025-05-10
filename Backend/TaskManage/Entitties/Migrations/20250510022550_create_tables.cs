using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class create_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_User", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    LogContent = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_TaskNode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskTitle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaskDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Deadline = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    TaskStatus = table.Column<int>(type: "int", nullable: true),
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
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
