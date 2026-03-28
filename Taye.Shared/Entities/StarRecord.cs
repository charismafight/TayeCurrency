using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Shared.Entities;

/// <summary>
/// 星星记录实体
/// </summary>
[Table("StarRecords")]
public class StarRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// 星星数量（正数=获得，负数=消费）
    /// </summary>
    [Required]
    public int StarCount { get; set; }

    /// <summary>
    /// 原因/选项
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 类型：获得(Gain) 或 消费(Spend) 或 惩罚（Punish)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 备注/详细说明
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 图片路径（服务器端存储路径）
    /// </summary>
    [MaxLength(500)]
    public string? ImagePath { get; set; }

    /// <summary>
    /// 图片文件名（用于显示）
    /// </summary>
    [MaxLength(200)]
    public string? ImageFileName { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [MaxLength(50)]
    public string? UserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
