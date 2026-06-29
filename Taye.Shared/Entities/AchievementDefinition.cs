using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taye.Shared.Entities;

[Table("AchievementDefinitions")]
public class AchievementDefinition
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

    [MaxLength(10)]
    public string? Icon { get; set; }

    [MaxLength(20)]
    public string? Category { get; set; }

    [Required]
    [MaxLength(200)]
    public string MatchReason { get; set; } = string.Empty;

    /// <summary>
    /// 是否隐藏成就
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// 隐藏成就触发条件（JSON）
    /// </summary>
    [MaxLength(500)]
    public string? TriggerConfig { get; set; }

    /// <summary>
    /// 里程碑配置（JSON）
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string MilestonesJson { get; set; } = "[]";

    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
