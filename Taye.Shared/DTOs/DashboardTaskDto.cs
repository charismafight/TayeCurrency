using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.DTOs
{
    public class DashboardTaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int CurrentProgress { get; set; }
        public int TargetProgress { get; set; }
        public int BonusStars { get; set; }
        public bool IsCompleted { get; set; }
        public string Type { get; set; } = "Weekly";
    }

}
