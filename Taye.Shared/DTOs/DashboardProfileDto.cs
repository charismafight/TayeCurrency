using System;

namespace Taye.Shared.DTOs;

/// <summary>
/// 英雄资料卡数据
/// </summary>
public class DashboardProfileDto
{
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string PlayerName { get; set; } = "Taye";

    /// <summary>
    /// 当前星星余额
    /// </summary>
    public int StarBalance { get; set; }

    /// <summary>
    /// 昨日结算时的星星余额
    /// </summary>
    public int YesterdayBalance { get; set; }

    /// <summary>
    /// 本周获得的总星星数
    /// </summary>
    public int WeeklyEarned { get; set; }

    /// <summary>
    /// 本周花费/消耗的总星星数 (正数)
    /// </summary>
    public int WeeklySpent { get; set; }

    /// <summary>
    /// 本周惩罚
    /// </summary>
    public int WeeklyPunished { get; set; }

    /// <summary>
    /// 当前等级名称 (如 "⚔️ 屠龙者")
    /// </summary>
    public string Rank { get; set; } = string.Empty;

    /// <summary>
    /// 下一等级名称 (如 "🌟 传奇英雄")
    /// </summary>
    public string NextRank { get; set; } = string.Empty;

    /// <summary>
    /// 距离下一等级的经验进度 (0-100)
    /// </summary>
    public int ExpPercent { get; set; }

    /// <summary>
    /// 历史累计获得的总星星数
    /// </summary>
    public int TotalStars { get; set; }
}
