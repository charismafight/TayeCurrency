using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Taye.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LevelNumber = table.Column<int>(type: "integer", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LevelIcon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RequiredStars = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StarCount = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StarRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StarCount = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageFileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarRecords", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LevelConfigs",
                columns: new[] { "Id", "IsActive", "LevelIcon", "LevelName", "LevelNumber", "RequiredStars", "SortOrder" },
                values: new object[,]
                {
                    { 1, true, "🌱", "初入世界", 1, 0, 1 },
                    { 2, true, "🔥", "勇敢探险家", 2, 50, 2 },
                    { 3, true, "⛏️", "矿工新手", 3, 120, 3 },
                    { 4, true, "🛡️", "铁甲勇士", 4, 210, 4 },
                    { 5, true, "💎", "钻石骑士", 5, 320, 5 },
                    { 6, true, "📖", "附魔大师", 6, 450, 6 },
                    { 7, true, "🔥", "下界探险家", 7, 600, 7 },
                    { 8, true, "🐉", "屠龙者", 8, 780, 8 },
                    { 9, true, "💀", "凋灵克星", 9, 990, 9 },
                    { 10, true, "✨", "信标建造者", 10, 1230, 10 },
                    { 11, true, "🔴", "红石工程师", 11, 1500, 11 },
                    { 12, true, "🧱", "传说工匠", 12, 1800, 12 }
                });

            migrationBuilder.InsertData(
                table: "ReasonTemplates",
                columns: new[] { "Id", "CreatedAt", "Icon", "IsActive", "Notes", "Reason", "SortOrder", "StarCount", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { -206, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📋", true, null, "老师反馈在学校违规违纪行为", 35, -12, "Punish", null },
                    { -205, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📉", true, null, "作业未完成或者各类老师评价得A-以下（不包括A-）", 34, -2, "Punish", null },
                    { -204, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🎒", true, null, "忘记带上课需要的文具、工具、书等等", 33, -3, "Punish", null },
                    { -203, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🧼", true, null, "起床或者晚上睡觉前忘记洗脸（或洗澡时忘记洗脸）", 32, -1, "Punish", null },
                    { -202, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🚽", true, null, "尿尿忘记冲厕所", 31, -1, "Punish", null },
                    { -201, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "😴", true, null, "晚上22:00前没有收好书包，洗漱完毕，换好睡衣，上自己的床", 30, -1, "Punish", null },
                    { -103, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📖", true, null, "雇佣爸爸读书（麦克狐1章，约10页）", 23, -1, "Spend", null },
                    { -102, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🎮", true, null, "玩游戏（每5分钟）", 22, -1, "Spend", null },
                    { -101, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🧸", true, null, "购买玩具（每1块钱）", 21, -1, "Spend", null },
                    { -100, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🍭", true, null, "购买零食（每1块钱）", 20, -1, "Spend", null },
                    { -11, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🍽️", true, null, "吃饭光盘", 11, 1, "Reward", null },
                    { -10, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🖋️", true, null, "写话（优秀）", 10, 3, "Reward", null },
                    { -9, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🌙", true, null, "晚上21:30前收好书包，洗漱完毕，换好睡衣，上自己的床", 9, 1, "Reward", null },
                    { -8, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📊", true, null, "语文考试A+或者数学考试获得99、98分（满分100）", 8, 6, "Reward", null },
                    { -7, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🏆", true, null, "考试获得100分（满分100）", 7, 12, "Reward", null },
                    { -6, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🌟", true, null, "老师在作业本上写表扬文字", 6, 1, "Reward", null },
                    { -5, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📚", true, null, "Khan完成一个unit的学习", 5, 6, "Reward", null },
                    { -4, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📝", true, null, "听写A+甲+", 4, 2, "Reward", null },
                    { -3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "📜", true, null, "默写古诗全对", 3, 2, "Reward", null },
                    { -2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "⭐", true, null, "老师表扬学校优秀表现", 2, 6, "Reward", null },
                    { -1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "✍️", true, null, "拼写本、写字书或者课练A+甲+", 1, 1, "Reward", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReasonTemplates_IsActive",
                table: "ReasonTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ReasonTemplates_SortOrder",
                table: "ReasonTemplates",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ReasonTemplates_Type",
                table: "ReasonTemplates",
                column: "Type");

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
                name: "LevelConfigs");

            migrationBuilder.DropTable(
                name: "ReasonTemplates");

            migrationBuilder.DropTable(
                name: "StarRecords");
        }
    }
}
