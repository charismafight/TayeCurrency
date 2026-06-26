using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.Entities
{
    [Table("LevelConfigs")]
    public class LevelConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 等级序号（1, 2, 3...）
        /// </summary>
        [Required]
        public int LevelNumber { get; set; }

        /// <summary>
        /// 等级名称（如 "星尘学徒"）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LevelName { get; set; } = string.Empty;

        /// <summary>
        /// 等级图标（如 "⚔️"）
        /// </summary>
        [MaxLength(10)]
        public string? LevelIcon { get; set; }

        /// <summary>
        /// 所需累计星星数（达到该数即升级）
        /// </summary>
        [Required]
        public int RequiredStars { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }
    }
}
