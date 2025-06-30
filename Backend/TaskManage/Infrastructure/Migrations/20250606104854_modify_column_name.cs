using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modify_column_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskNodeTaskNode_T_TaskNode_TaskNodeId",
                table: "TaskNodeTaskNode");

            migrationBuilder.RenameColumn(
                name: "TaskNodeId",
                table: "TaskNodeTaskNode",
                newName: "SuccessorNodesId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskNodeTaskNode_TaskNodeId",
                table: "TaskNodeTaskNode",
                newName: "IX_TaskNodeTaskNode_SuccessorNodesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskNodeTaskNode_T_TaskNode_SuccessorNodesId",
                table: "TaskNodeTaskNode",
                column: "SuccessorNodesId",
                principalTable: "T_TaskNode",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskNodeTaskNode_T_TaskNode_SuccessorNodesId",
                table: "TaskNodeTaskNode");

            migrationBuilder.RenameColumn(
                name: "SuccessorNodesId",
                table: "TaskNodeTaskNode",
                newName: "TaskNodeId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskNodeTaskNode_SuccessorNodesId",
                table: "TaskNodeTaskNode",
                newName: "IX_TaskNodeTaskNode_TaskNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskNodeTaskNode_T_TaskNode_TaskNodeId",
                table: "TaskNodeTaskNode",
                column: "TaskNodeId",
                principalTable: "T_TaskNode",
                principalColumn: "Id");
        }
    }
}
