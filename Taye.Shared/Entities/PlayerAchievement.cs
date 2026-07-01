using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Shared.Entities;

[Table("PlayerAchievements")]
public class PlayerAchievement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string AchievementId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int CurrentCount { get; set; }

    /// <summary>
    /// 已解锁的最高里程碑索引（-1 表示未解锁）
    /// </summary>
    public int LastMilestoneIndex { get; set; } = -1;

    /// <summary>
    /// 是否已解锁
    /// </summary>
    public bool IsUnlocked => LastMilestoneIndex >= 0;

    [MaxLength(50)]
    public string? UserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
