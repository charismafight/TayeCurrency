using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;

namespace Taye.WebAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 数据库表映射
    public DbSet<StarRecord> StarRecords { get; set; }
    public DbSet<ReasonTemplate> ReasonTemplates { get; set; }

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

        // 种子数据：初始化默认模板
        var fixedDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<ReasonTemplate>().HasData(
            // 奖励类
            new ReasonTemplate { Id = -1, Reason = "拼写本、写字书或者课练A+甲+", StarCount = 1, Type = "Reward", SortOrder = 1, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -2, Reason = "老师表扬学校优秀表现", StarCount = 6, Type = "Reward", SortOrder = 2, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -3, Reason = "默写古诗全对", StarCount = 2, Type = "Reward", SortOrder = 3, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -4, Reason = "听写A+甲+", StarCount = 2, Type = "Reward", SortOrder = 4, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -5, Reason = "Khan完成一个unit的学习", StarCount = 6, Type = "Reward", SortOrder = 5, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -6, Reason = "老师在作业本上写表扬文字", StarCount = 1, Type = "Reward", SortOrder = 6, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -7, Reason = "考试获得100分（满分100）", StarCount = 12, Type = "Reward", SortOrder = 7, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -8, Reason = "语文考试A+或者数学考试获得99、98分（满分100）", StarCount = 6, Type = "Reward", SortOrder = 8, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -9, Reason = "晚上21:30前收好书包，洗漱完毕，换好睡衣，上自己的床", StarCount = 1, Type = "Reward", SortOrder = 9, IsActive = true, CreatedAt = fixedDate },

            // 花费类
            new ReasonTemplate { Id = -100, Reason = "购买零食（每1块钱）", StarCount = -1, Type = "Spend", SortOrder = 10, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -101, Reason = "购买玩具（每1块钱）", StarCount = -1, Type = "Spend", SortOrder = 11, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -102, Reason = "玩游戏（每5分钟）", StarCount = -1, Type = "Spend", SortOrder = 12, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -103, Reason = "雇佣爸爸读书（麦克狐1章，约10页）", StarCount = -1, Type = "Spend", SortOrder = 13, IsActive = true, CreatedAt = fixedDate },

            // 惩罚类
            new ReasonTemplate { Id = -201, Reason = "晚上22:00前没有收好书包，洗漱完毕，换好睡衣，上自己的床", StarCount = -1, Type = "Punish", SortOrder = 13, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -202, Reason = "尿尿忘记冲厕所", StarCount = -1, Type = "Punish", SortOrder = 14, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -203, Reason = "起床或者晚上睡觉前忘记洗脸（或洗澡时忘记洗脸）", StarCount = -1, Type = "Punish", SortOrder = 15, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -204, Reason = "忘记带上课需要的文具、工具、书等等", StarCount = -3, Type = "Punish", SortOrder = 16, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -205, Reason = "作业未完成或者各类老师评价得A-以下（不包括A-）", StarCount = -2, Type = "Punish", SortOrder = 17, IsActive = true, CreatedAt = fixedDate },
            new ReasonTemplate { Id = -206, Reason = "老师反馈在学校违规违纪行为", StarCount = -12, Type = "Punish", SortOrder = 18, IsActive = true, CreatedAt = fixedDate }
        );
    }
}
