using System;

namespace Taye.Shared.DTOs;

/// <summary>
/// 星星记录 DTO（用于展示）
/// </summary>
public class StarRecordDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int StarCount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageFileName { get; set; }

    // 辅助属性
    public string DisplayDate => Date.ToString("yyyy-MM-dd HH:mm:ss");
    public string DisplayStar => Type == "Gain" ? $"+{StarCount}" : $"{StarCount}";
    public string DisplayType => Type == "Gain" ? "获得" : "消费";
    public string? ImageUrl => !string.IsNullOrEmpty(ImagePath) ? $"/api/files/{ImagePath}" : null;
}
