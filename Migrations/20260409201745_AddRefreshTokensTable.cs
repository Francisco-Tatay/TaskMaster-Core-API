using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerPro.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokenEntities_User_UserId",
                table: "RefreshTokenEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokenEntities",
                table: "RefreshTokenEntities");

            migrationBuilder.RenameTable(
                name: "RefreshTokenEntities",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokenEntities_UserId",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RefreshToken",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "RefreshToken",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_User_UserId",
                table: "RefreshToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_User_UserId",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshTokenEntities");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshTokenEntities",
                newName: "IX_RefreshTokenEntities_UserId");

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Title",
                keyValue: null,
                column: "Title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokenEntities",
                table: "RefreshTokenEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokenEntities_User_UserId",
                table: "RefreshTokenEntities",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
