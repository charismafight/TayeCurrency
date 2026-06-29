using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Taye.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievementSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AchievementId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MatchReason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    TriggerConfig = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MilestonesJson = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AchievementId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentCount = table.Column<int>(type: "integer", nullable: false),
                    LastMilestoneIndex = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAchievements", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AchievementDefinitions",
                columns: new[] { "Id", "AchievementId", "Category", "Icon", "IsActive", "IsHidden", "MatchReason", "MilestonesJson", "Name", "SortOrder", "TriggerConfig" },
                values: new object[,]
                {
                    { -103, "all_around", "隐藏", "🌟", true, true, "", "[{\"count\":1,\"title\":\"解锁\",\"bonus\":5}]", "全能小选手", 103, "{}" },
                    { -102, "self_discipline", "隐藏", "⏰", true, true, "", "[{\"count\":1,\"title\":\"解锁\",\"bonus\":5}]", "自律小达人", 102, "{}" },
                    { -101, "bookworm", "隐藏", "📖", true, true, "", "[{\"count\":1,\"title\":\"解锁\",\"bonus\":3}]", "读书人", 101, "{}" },
                    { -100, "early_bird", "隐藏", "🌅", true, true, "", "[{\"count\":1,\"title\":\"解锁\",\"bonus\":2}]", "早起鸟", 100, "{}" },
                    { -11, "homework_pro", "生活", "📋", true, false, "主动完成常规作业", "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]", "作业小先锋", 11, null },
                    { -10, "tidy_table", "生活", "🧹", true, false, "吃完饭清理桌面并收碗", "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]", "整理小能手", 10, null },
                    { -9, "clean_plate", "生活", "🍽️", true, false, "吃饭光盘", "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]", "光盘行动", 9, null },
                    { -8, "bedtime", "生活", "🌙", true, false, "晚上21:30前", "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]", "早睡达人", 8, null },
                    { -7, "write", "学业", "🖋️", true, false, "写话（优秀）", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]", "写话之星", 7, null },
                    { -6, "khan", "学业", "📚", true, false, "完成Khan unit", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]", "Khan学霸", 6, null },
                    { -5, "praise", "学业", "🌟", true, false, "老师在作业本上写表扬文字", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]", "表扬信", 5, null },
                    { -4, "homework", "学业", "✍️", true, false, "拼写本、写字书或者课练A+", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]", "作业精英", 4, null },
                    { -3, "recite", "学业", "📜", true, false, "默写古诗全对", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]", "古诗达人", 3, null },
                    { -2, "dictation", "学业", "📝", true, false, "听写A+甲+", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]", "听写大师", 2, null },
                    { -1, "perfect_score", "学业", "🏆", true, false, "考试获得100分", "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]", "满分猎人", 1, null }
                });

            migrationBuilder.InsertData(
                table: "ReasonTemplates",
                columns: new[] { "Id", "CreatedAt", "Icon", "IsActive", "Notes", "Reason", "SortOrder", "StarCount", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { -14, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📚", true, null, "读完一本书并写一篇心得", 14, 12, "Reward", null },
                    { -13, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📋", true, null, "主动完成常规作业", 13, 1, "Reward", null },
                    { -12, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🧹", true, null, "吃完饭清理桌面并收碗", 12, 1, "Reward", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_AchievementId",
                table: "AchievementDefinitions",
                column: "AchievementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_IsActive",
                table: "AchievementDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_AchievementId_UserId",
                table: "PlayerAchievements",
                columns: new[] { "AchievementId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_UserId",
                table: "PlayerAchievements",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementDefinitions");

            migrationBuilder.DropTable(
                name: "PlayerAchievements");

            migrationBuilder.DeleteData(
                table: "ReasonTemplates",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "ReasonTemplates",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.DeleteData(
                table: "ReasonTemplates",
                keyColumn: "Id",
                keyValue: -12);
        }
    }
}
