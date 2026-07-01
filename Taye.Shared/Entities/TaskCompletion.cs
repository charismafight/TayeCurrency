using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Shared.Entities;

/// <summary>
/// 任务完成记录（支持每日/每周等周期）
/// </summary>
[Table("TaskCompletions")]
public class TaskCompletion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 任务标识（如 "bedtime", "clean_plate"）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// 任务名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 周期类型：Daily / Weekly / Monthly
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PeriodType { get; set; } = "Daily";

    /// <summary>
    /// 周期键值（如 "2026-06-26" 或 "2026-W26"）
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PeriodKey { get; set; } = string.Empty;

    /// <summary>
    /// 目标次数
    /// </summary>
    [Required]
    public int TargetCount { get; set; }

    /// <summary>
    /// 实际完成次数
    /// </summary>
    [Required]
    public int CompletedCount { get; set; }

    /// <summary>
    /// 是否达标
    /// </summary>
    [Required]
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 奖励是否已领取
    /// </summary>
    [Required]
    public bool BonusEarned { get; set; }

    /// <summary>
    /// 用户ID（多用户预留）
    /// </summary>
    [MaxLength(50)]
    public string? UserId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
