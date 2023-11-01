using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileExplorer.Migrations;

/// <inheritdoc />
public partial class ApplyConfiguration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserFiles",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            table: "UserFiles",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "GETDATE()",
            oldClrType: typeof(DateTime),
            oldType: "datetime2");

        migrationBuilder.CreateIndex(
            name: "IX_UserFiles_Name",
            table: "UserFiles",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
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

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            table: "UserFiles",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldDefaultValueSql: "GETDATE()");
    }
}
