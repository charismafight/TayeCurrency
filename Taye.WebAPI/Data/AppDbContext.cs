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

        // 可选：添加种子数据（示例）
        // SeedData(modelBuilder);
    }

    // 可选：种子数据
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StarRecord>().HasData(
            new StarRecord
            {
                Id = 1,
                Date = DateTime.Today,
                StarCount = 5,
                Reason = "示例：完成作业",
                Type = "Gain",
                Notes = "这是示例数据",
                CreatedAt = DateTime.UtcNow
            },
            new StarRecord
            {
                Id = 2,
                Date = DateTime.Today,
                StarCount = 2,
                Reason = "示例：玩游戏",
                Type = "Spend",
                Notes = "这是示例数据",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
