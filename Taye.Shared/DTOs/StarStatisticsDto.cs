using System;

namespace Taye.Shared.DTOs;

/// <summary>
/// 星星统计 DTO
/// </summary>
public class StarStatisticsDto
{
    /// <summary>
    /// 总获得星星数
    /// </summary>
    public int TotalGain { get; set; }

    /// <summary>
    /// 总消费星星数
    /// </summary>
    public int TotalSpend { get; set; }

    /// <summary>
    /// 当前剩余星星数
    /// </summary>
    public int CurrentBalance => TotalGain - TotalSpend;

    /// <summary>
    /// 今日获得星星数
    /// </summary>
    public int TodayGain { get; set; }

    /// <summary>
    /// 今日消费星星数
    /// </summary>
    public int TodaySpend { get; set; }

    /// <summary>
    /// 本周获得星星数
    /// </summary>
    public int WeekGain { get; set; }

    /// <summary>
    /// 本周消费星星数
    /// </summary>
    public int WeekSpend { get; set; }

    /// <summary>
    /// 本月获得星星数
    /// </summary>
    public int MonthGain { get; set; }

    /// <summary>
    /// 本月消费星星数
    /// </summary>
    public int MonthSpend { get; set; }

    /// <summary>
    /// 最近7天数据（用于图表）
    /// </summary>
    public List<DailyStarDto> RecentDays { get; set; } = new();
}

/// <summary>
/// 每日星星统计
/// </summary>
public class DailyStarDto
{
    public DateTime Date { get; set; }
    public int Gain { get; set; }
    public int Spend { get; set; }
    public int Net => Gain - Spend;
}
