using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.Entities
{
    [Table("PlayerConfigs")]
    public class PlayerConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 玩家名称（如 Taye）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// 头像 URL（可选）
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 当前星星余额
        /// </summary>
        public int StarBalance { get; set; }

        /// <summary>
        /// 累计获得星星数
        /// </summary>
        public int TotalStarsEarned { get; set; }

        /// <summary>
        /// 是否是当前活跃玩家
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
