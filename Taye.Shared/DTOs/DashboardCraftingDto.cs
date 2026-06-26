using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.DTOs
{
    public class DashboardCraftingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Cost { get; set; }
        public bool Available { get; set; }
        public string Status { get; set; } = string.Empty; // "craftable" | "insufficient"
    }
}
