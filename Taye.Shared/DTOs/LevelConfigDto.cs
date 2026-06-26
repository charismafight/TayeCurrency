using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.DTOs
{
    /// <summary>
    /// 等级配置 DTO
    /// </summary>
    public class LevelConfigDto
    {
        public int Id { get; set; }
        public int LevelNumber { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public string? LevelIcon { get; set; }
        public int RequiredStars { get; set; }
        public bool IsActive { get; set; }
    }
}
