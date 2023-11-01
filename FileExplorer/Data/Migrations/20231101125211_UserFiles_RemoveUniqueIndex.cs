﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileExplorer.Migrations
{
    /// <inheritdoc />
    public partial class UserFiles_RemoveUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserFiles_Name",
                table: "UserFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserFiles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserFiles_Name",
                table: "UserFiles",
                column: "Name",
                unique: true);
        }
    }
}
