using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Shared.Entities;

/// <summary>
/// 原因模板实体（原因和星星数量的关系）
/// </summary>
[Table("ReasonTemplates")]
public class ReasonTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 原因/选项名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 星星数量（正数=奖励，负数=花费/惩罚）
    /// </summary>
    [Required]
    public int StarCount { get; set; }

    /// <summary>
    /// 类型：奖励(Reward)、花费(Spend)、惩罚(Punish)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 备注说明
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 排序序号（越小越靠前）
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
