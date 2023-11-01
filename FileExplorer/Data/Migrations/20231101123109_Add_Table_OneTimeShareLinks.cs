using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileExplorer.Migrations;

/// <inheritdoc />
public partial class Add_Table_OneTimeShareLinks : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OneTimeShareLinks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsUsed = table.Column<bool>(type: "bit", nullable: false),
                FileIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OneTimeShareLinks", x => x.Id);
                table.ForeignKey(
                    name: "FK_OneTimeShareLinks_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OneTimeShareLinks_UserId",
            table: "OneTimeShareLinks",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OneTimeShareLinks");
    }
}
