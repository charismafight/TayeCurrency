using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taye.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFileNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StarRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StarCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageFileName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarRecords_Date",
                table: "StarRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_StarRecords_Date_UserId",
                table: "StarRecords",
                columns: new[] { "Date", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_StarRecords_Type",
                table: "StarRecords",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_StarRecords_UserId",
                table: "StarRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StarRecords");
        }
    }
}
