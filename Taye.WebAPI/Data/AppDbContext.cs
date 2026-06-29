using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;

namespace Taye.WebAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            Database.EnsureCreated();
        }
    }

    // 数据库表映射
    public DbSet<StarRecord> StarRecords { get; set; }
    public DbSet<ReasonTemplate> ReasonTemplates { get; set; }
    public DbSet<LevelConfig> LevelConfigs { get; set; }
    public DbSet<TaskCompletion> TaskCompletions { get; set; }
    public DbSet<AchievementDefinition> AchievementDefinitions { get; set; }
    public DbSet<PlayerAchievement> PlayerAchievements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 StarRecord 实体
        modelBuilder.Entity<StarRecord>(entity =>
        {
            // 表名
            entity.ToTable("StarRecords");

            // 主键
            entity.HasKey(e => e.Id);

            // 索引：按日期查询优化
            entity.HasIndex(e => e.Date);

            // 索引：按类型查询优化
            entity.HasIndex(e => e.Type);

            // 索引：按用户ID查询（多用户支持）
            entity.HasIndex(e => e.UserId);

            // 组合索引：日期 + 用户（常用查询）
            entity.HasIndex(e => new { e.Date, e.UserId });

            // 配置字段
            entity.Property(e => e.StarCount)
                  .IsRequired();

            entity.Property(e => e.Reason)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Type)
                  .IsRequired()
                  .HasMaxLength(10);

            entity.Property(e => e.Notes)
                  .HasMaxLength(500);

            entity.Property(e => e.UserId)
                  .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // 软删除过滤：默认不查询已删除的记录
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // 配置 ReasonTemplate
        modelBuilder.Entity<ReasonTemplate>(entity =>
        {
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.SortOrder);
        });

        modelBuilder.Entity<TaskCompletion>(entity =>
        {
            entity.HasIndex(e => new { e.TaskId, e.PeriodKey }).IsUnique();
            entity.HasIndex(e => e.PeriodKey);
            entity.HasIndex(e => e.PeriodType);
        });

        modelBuilder.Entity<AchievementDefinition>(entity =>
        {
            entity.HasIndex(e => e.AchievementId).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<PlayerAchievement>(entity =>
        {
            entity.HasIndex(e => new { e.AchievementId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.UserId);
        });

        // 种子数据：初始化默认模板
        var fixedDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<ReasonTemplate>().HasData(
            // 奖励类
            new ReasonTemplate { Id = -1, Reason = "拼写本、写字书或者课练A+甲+", StarCount = 1, Type = "Reward", SortOrder = 1, IsActive = true, Icon = "✍️", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -2, Reason = "老师表扬学校优秀表现", StarCount = 6, Type = "Reward", SortOrder = 2, IsActive = true, Icon = "⭐", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -3, Reason = "默写古诗全对", StarCount = 2, Type = "Reward", SortOrder = 3, IsActive = true, Icon = "📜", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -4, Reason = "听写A+甲+", StarCount = 2, Type = "Reward", SortOrder = 4, IsActive = true, Icon = "📝", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -5, Reason = "Khan完成一个unit的学习", StarCount = 6, Type = "Reward", SortOrder = 5, IsActive = true, Icon = "📚", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -6, Reason = "老师在作业本上写表扬文字", StarCount = 1, Type = "Reward", SortOrder = 6, IsActive = true, Icon = "🌟", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -7, Reason = "考试获得100分（满分100）", StarCount = 12, Type = "Reward", SortOrder = 7, IsActive = true, Icon = "🏆", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -8, Reason = "语文考试A+或者数学考试获得99、98分（满分100）", StarCount = 6, Type = "Reward", SortOrder = 8, IsActive = true, Icon = "📊", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -9, Reason = "晚上21:30前收好书包，洗漱完毕，换好睡衣，上自己的床", StarCount = 1, Type = "Reward", SortOrder = 9, IsActive = true, Icon = "🌙", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -10, Reason = "写话（优秀）", StarCount = 3, Type = "Reward", SortOrder = 10, IsActive = true, Icon = "🖋️", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -11, Reason = "吃饭光盘", StarCount = 1, Type = "Reward", SortOrder = 11, IsActive = true, Icon = "🍽️", CreatedAt = fixedDate },

            // 花费类
            new ReasonTemplate { Id = -100, Reason = "兑换零食（每1块钱）", StarCount = -1, Type = "Spend", SortOrder = 20, IsActive = true, Icon = "🍭", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -101, Reason = "兑换玩具（每1块钱）", StarCount = -1, Type = "Spend", SortOrder = 21, IsActive = true, Icon = "🧸", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -102, Reason = "兑换游戏时长（每5分钟）", StarCount = -1, Type = "Spend", SortOrder = 22, IsActive = true, Icon = "🎮", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -103, Reason = "雇佣爸爸读书（每5分钟）", StarCount = -1, Type = "Spend", SortOrder = 23, IsActive = true, Icon = "📖", CreatedAt = fixedDate },

            // 惩罚类
            new ReasonTemplate { Id = -201, Reason = "晚上22:00前没有收好书包，洗漱完毕，换好睡衣，上自己的床", StarCount = -1, Type = "Punish", SortOrder = 30, IsActive = true, Icon = "😴", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -202, Reason = "尿尿忘记冲厕所", StarCount = -1, Type = "Punish", SortOrder = 31, IsActive = true, Icon = "🚽", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -203, Reason = "起床或者晚上睡觉前忘记洗脸（或洗澡时忘记洗脸）", StarCount = -1, Type = "Punish", SortOrder = 32, IsActive = true, Icon = "🧼", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -204, Reason = "忘记带上课需要的文具、工具、书等等", StarCount = -3, Type = "Punish", SortOrder = 33, IsActive = true, Icon = "🎒", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -205, Reason = "作业未完成或者各类老师评价得A-以下（不包括A-）", StarCount = -2, Type = "Punish", SortOrder = 34, IsActive = true, Icon = "📉", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -206, Reason = "老师反馈在学校违规违纪行为", StarCount = -12, Type = "Punish", SortOrder = 35, IsActive = true, Icon = "📋", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -12, Reason = "吃完饭清理桌面并收碗", StarCount = 1, Type = "Reward", SortOrder = 12, IsActive = true, Icon = "🧹", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -13, Reason = "主动完成常规作业", StarCount = 1, Type = "Reward", SortOrder = 13, IsActive = true, Icon = "📋", CreatedAt = fixedDate },
            new ReasonTemplate { Id = -14, Reason = "读完一本书并写一篇心得", StarCount = 12, Type = "Reward", SortOrder = 14, IsActive = true, Icon = "📚", CreatedAt = fixedDate });

        // ========== LevelConfig 种子数据 ==========
        modelBuilder.Entity<LevelConfig>().HasData(
            new LevelConfig { Id = 1, LevelNumber = 1, LevelName = "初入世界", LevelIcon = "🌱", RequiredStars = 0, IsActive = true, SortOrder = 1 },
            new LevelConfig { Id = 2, LevelNumber = 2, LevelName = "勇敢探险家", LevelIcon = "🔥", RequiredStars = 50, IsActive = true, SortOrder = 2 },
            new LevelConfig { Id = 3, LevelNumber = 3, LevelName = "矿工新手", LevelIcon = "⛏️", RequiredStars = 120, IsActive = true, SortOrder = 3 },
            new LevelConfig { Id = 4, LevelNumber = 4, LevelName = "铁甲勇士", LevelIcon = "🛡️", RequiredStars = 210, IsActive = true, SortOrder = 4 },
            new LevelConfig { Id = 5, LevelNumber = 5, LevelName = "钻石骑士", LevelIcon = "💎", RequiredStars = 320, IsActive = true, SortOrder = 5 },
            new LevelConfig { Id = 6, LevelNumber = 6, LevelName = "附魔大师", LevelIcon = "📖", RequiredStars = 450, IsActive = true, SortOrder = 6 },
            new LevelConfig { Id = 7, LevelNumber = 7, LevelName = "下界探险家", LevelIcon = "🔥", RequiredStars = 600, IsActive = true, SortOrder = 7 },
            new LevelConfig { Id = 8, LevelNumber = 8, LevelName = "屠龙者", LevelIcon = "🐉", RequiredStars = 780, IsActive = true, SortOrder = 8 },
            new LevelConfig { Id = 9, LevelNumber = 9, LevelName = "凋灵克星", LevelIcon = "💀", RequiredStars = 990, IsActive = true, SortOrder = 9 },
            new LevelConfig { Id = 10, LevelNumber = 10, LevelName = "信标建造者", LevelIcon = "✨", RequiredStars = 1230, IsActive = true, SortOrder = 10 },
            new LevelConfig { Id = 11, LevelNumber = 11, LevelName = "红石工程师", LevelIcon = "🔴", RequiredStars = 1500, IsActive = true, SortOrder = 11 },
            new LevelConfig { Id = 12, LevelNumber = 12, LevelName = "传说工匠", LevelIcon = "🧱", RequiredStars = 1800, IsActive = true, SortOrder = 12 }
        );

        // 里程碑成就
        modelBuilder.Entity<AchievementDefinition>().HasData(
            new AchievementDefinition
            {
                Id = -1,
                AchievementId = "perfect_score",
                Name = "满分猎人",
                Icon = "🏆",
                Category = "学业",
                MatchReason = "考试获得100分",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 1
            },
            new AchievementDefinition
            {
                Id = -2,
                AchievementId = "dictation",
                Name = "听写大师",
                Icon = "📝",
                Category = "学业",
                MatchReason = "听写A+甲+",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 2
            },
            new AchievementDefinition
            {
                Id = -3,
                AchievementId = "recite",
                Name = "古诗达人",
                Icon = "📜",
                Category = "学业",
                MatchReason = "默写古诗全对",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 3
            },
            new AchievementDefinition
            {
                Id = -4,
                AchievementId = "homework",
                Name = "作业精英",
                Icon = "✍️",
                Category = "学业",
                MatchReason = "拼写本、写字书或者课练A+",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":5,\"title\":\"达人\",\"bonus\":3},{\"count\":10,\"title\":\"大师\",\"bonus\":5},{\"count\":20,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 4
            },
            new AchievementDefinition
            {
                Id = -5,
                AchievementId = "praise",
                Name = "表扬信",
                Icon = "🌟",
                Category = "学业",
                MatchReason = "老师在作业本上写表扬文字",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]",
                SortOrder = 5
            },
            new AchievementDefinition
            {
                Id = -6,
                AchievementId = "khan",
                Name = "Khan学霸",
                Icon = "📚",
                Category = "学业",
                MatchReason = "完成Khan unit",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]",
                SortOrder = 6
            },
            new AchievementDefinition
            {
                Id = -7,
                AchievementId = "write",
                Name = "写话之星",
                Icon = "🖋️",
                Category = "学业",
                MatchReason = "写话（优秀）",
                MilestonesJson = "[{\"count\":1,\"title\":\"新秀\",\"bonus\":1},{\"count\":3,\"title\":\"达人\",\"bonus\":3},{\"count\":5,\"title\":\"大师\",\"bonus\":5}]",
                SortOrder = 7
            },
            new AchievementDefinition
            {
                Id = -8,
                AchievementId = "bedtime",
                Name = "早睡达人",
                Icon = "🌙",
                Category = "生活",
                MatchReason = "晚上21:30前",
                MilestonesJson = "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 8
            },
            new AchievementDefinition
            {
                Id = -9,
                AchievementId = "clean_plate",
                Name = "光盘行动",
                Icon = "🍽️",
                Category = "生活",
                MatchReason = "吃饭光盘",
                MilestonesJson = "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 9
            },
            new AchievementDefinition
            {
                Id = -10,
                AchievementId = "tidy_table",
                Name = "整理小能手",
                Icon = "🧹",
                Category = "生活",
                MatchReason = "吃完饭清理桌面并收碗",
                MilestonesJson = "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 10
            },
            new AchievementDefinition
            {
                Id = -11,
                AchievementId = "homework_pro",
                Name = "作业小先锋",
                Icon = "📋",
                Category = "生活",
                MatchReason = "主动完成常规作业",
                MilestonesJson = "[{\"count\":3,\"title\":\"新秀\",\"bonus\":1},{\"count\":7,\"title\":\"达人\",\"bonus\":3},{\"count\":14,\"title\":\"大师\",\"bonus\":5},{\"count\":30,\"title\":\"传奇\",\"bonus\":8}]",
                SortOrder = 11
            }
        );

        // 隐藏成就
        modelBuilder.Entity<AchievementDefinition>().HasData(
            new AchievementDefinition
            {
                Id = -100,
                AchievementId = "early_bird",
                Name = "早起鸟",
                Icon = "🌅",
                Category = "隐藏",
                MatchReason = "",
                IsHidden = true,
                TriggerConfig = "{}",
                MilestonesJson = "[{\"count\":1,\"title\":\"解锁\",\"bonus\":2}]",
                SortOrder = 100
            },
            new AchievementDefinition
            {
                Id = -101,
                AchievementId = "bookworm",
                Name = "读书人",
                Icon = "📖",
                Category = "隐藏",
                MatchReason = "",
                IsHidden = true,
                TriggerConfig = "{}",
                MilestonesJson = "[{\"count\":1,\"title\":\"解锁\",\"bonus\":3}]",
                SortOrder = 101
            },
            new AchievementDefinition
            {
                Id = -102,
                AchievementId = "self_discipline",
                Name = "自律小达人",
                Icon = "⏰",
                Category = "隐藏",
                MatchReason = "",
                IsHidden = true,
                TriggerConfig = "{}",
                MilestonesJson = "[{\"count\":1,\"title\":\"解锁\",\"bonus\":5}]",
                SortOrder = 102
            },
            new AchievementDefinition
            {
                Id = -103,
                AchievementId = "all_around",
                Name = "全能小选手",
                Icon = "🌟",
                Category = "隐藏",
                MatchReason = "",
                IsHidden = true,
                TriggerConfig = "{}",
                MilestonesJson = "[{\"count\":1,\"title\":\"解锁\",\"bonus\":5}]",
                SortOrder = 103
            }
        );
    }
}
