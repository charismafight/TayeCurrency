using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taye.Shared.DTOs
{
    public class DashboardActivityDto
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public int StarCount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }
    }
}
