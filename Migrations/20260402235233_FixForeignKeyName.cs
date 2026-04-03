using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerPro.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_User_UserEntityId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_UserEntityId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "Task");

            migrationBuilder.CreateIndex(
                name: "IX_Task_UserId",
                table: "Task",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_User_UserId",
                table: "Task",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_User_UserId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_UserId",
                table: "Task");

            migrationBuilder.AddColumn<int>(
                name: "UserEntityId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Task_UserEntityId",
                table: "Task",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_User_UserEntityId",
                table: "Task",
                column: "UserEntityId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
