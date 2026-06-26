using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;
using Taye.WebAPI.Services;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Taye.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardController> _logger;
    private readonly IReasonTemplateService _reasonTemplateService;
    private readonly ILevelConfigService _levelConfigService;

    public DashboardController(
        AppDbContext context,
        ILogger<DashboardController> logger,
        IReasonTemplateService reasonTemplateService,
        ILevelConfigService levelConfigService)
    {
        _context = context;
        _logger = logger;
        _reasonTemplateService = reasonTemplateService;
        _levelConfigService = levelConfigService;
    }

    /// <summary>
    /// 获取英雄资料卡数据
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(APIResponse<DashboardProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<DashboardProfileDto>>> GetProfile(
        [FromQuery] string? userId = null)
    {
        try
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            if (startOfWeek > today) startOfWeek = startOfWeek.AddDays(-7);

            var query = _context.StarRecords.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            // 总累计获得（所有正的）
            var totalStars = await query
                .Where(r => !r.IsDeleted && r.StarCount > 0)
                .SumAsync(r => r.StarCount);

            // 当前余额
            var starBalance = await query
                .Where(r => !r.IsDeleted)
                .SumAsync(r => r.StarCount);

            // 昨日余额（昨天的数据）
            var yesterday = today.AddDays(-1);
            var yesterdayBalance = await query
                .Where(r => !r.IsDeleted && r.Date < today)
                .SumAsync(r => r.StarCount);

            // 本周获得
            var weeklyEarned = await query
                .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.StarCount > 0)
                .SumAsync(r => r.StarCount);

            // 本周消费（取绝对值）
            var weeklySpent = await query
                .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.StarCount < 0)
                .SumAsync(r => -r.StarCount);

            var weeklyPunished = await query
    .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.Type == "Punish")
    .SumAsync(r => Math.Abs(r.StarCount));

            // 计算等级
            var (rank, nextRank, expPercent) = await _levelConfigService.CalculateLevelAsync(totalStars);

            var dto = new DashboardProfileDto
            {
                PlayerName = "Taye", // 后续可从配置表读取
                StarBalance = starBalance,
                YesterdayBalance = yesterdayBalance,
                WeeklyEarned = weeklyEarned,
                WeeklySpent = weeklySpent,
                WeeklyPunished = weeklyPunished,
                Rank = rank,
                NextRank = nextRank,
                ExpPercent = expPercent,
                TotalStars = totalStars
            };

            return Ok(APIResponse<DashboardProfileDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取资料卡数据失败");
            return StatusCode(500, APIResponse<DashboardProfileDto>.Fail("获取数据失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 获取今日任务列表
    /// </summary>
    [HttpGet("tasks")]
    [ProducesResponseType(typeof(APIResponse<List<DashboardTaskDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<DashboardTaskDto>>>> GetTasks(
        [FromQuery] string? userId = null)
    {
        try
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            if (startOfWeek > today) startOfWeek = startOfWeek.AddDays(-7);

            var query = _context.StarRecords.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            // 获取本周所有 Reward 类型的记录
            var weekRecords = await query
                .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.Date <= today)
                .GroupBy(r => r.Reason)
                .Select(g => new
                {
                    Reason = g.Key,
                    Count = g.Count(),
                    TotalStars = g.Sum(r => r.StarCount)
                })
                .ToListAsync();

            // 获取本周所有 Spend 类型的记录
            var spendRecords = await query
                .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.Date <= today && r.StarCount < 0)
                .GroupBy(r => r.Reason)
                .Select(g => new
                {
                    Reason = g.Key,
                    Count = g.Count(),
                    TotalStars = g.Sum(r => -r.StarCount)
                })
                .ToListAsync();

            var tasks = new List<DashboardTaskDto>();

            // 1. 早睡达人：本周 21:30前上床 达到 5 天
            var earlyBedCount = weekRecords
                .Where(r => r.Reason.Contains("21:30") || r.Reason.Contains("上床"))
                .Sum(r => r.Count);
            tasks.Add(new DashboardTaskDto
            {
                Id = 1,
                Name = "早睡达人",
                Description = "本周 21:30 前上床睡觉 5 天",
                Icon = "🌙",
                CurrentProgress = Math.Min(earlyBedCount, 5),
                TargetProgress = 5,
                BonusStars = 1,
                IsCompleted = earlyBedCount >= 5,
                Type = "Weekly"
            });

            // 2. 光盘行动：本周 吃饭光盘 达到 7 次
            var cleanPlateCount = weekRecords
                .Where(r => r.Reason.Contains("光盘") || r.Reason.Contains("吃饭"))
                .Sum(r => r.Count);
            tasks.Add(new DashboardTaskDto
            {
                Id = 2,
                Name = "光盘行动",
                Description = "本周吃完每顿饭 7 次",
                Icon = "🍽️",
                CurrentProgress = Math.Min(cleanPlateCount, 7),
                TargetProgress = 7,
                BonusStars = 1,
                IsCompleted = cleanPlateCount >= 7,
                Type = "Weekly"
            });

            // 3. 满分猎人：本周考试100分达到 2 次
            var perfectScoreCount = weekRecords
                .Where(r => r.Reason.Contains("100分") || r.Reason.Contains("满分"))
                .Sum(r => r.Count);
            tasks.Add(new DashboardTaskDto
            {
                Id = 3,
                Name = "满分猎人",
                Description = "本周考试获得 100 分 2 次",
                Icon = "📝",
                CurrentProgress = Math.Min(perfectScoreCount, 2),
                TargetProgress = 2,
                BonusStars = 2,
                IsCompleted = perfectScoreCount >= 2,
                Type = "Weekly"
            });

            // 4. 无违纪周：本周没有违规记录
            var violationCount = spendRecords
                .Where(r => r.Reason.Contains("违规") || r.Reason.Contains("违纪"))
                .Sum(r => r.Count);
            tasks.Add(new DashboardTaskDto
            {
                Id = 4,
                Name = "无违纪周",
                Description = "本周没有学校违规记录",
                Icon = "🛡️",
                CurrentProgress = violationCount == 0 ? 1 : 0,
                TargetProgress = 1,
                BonusStars = 1,
                IsCompleted = violationCount == 0,
                Type = "Weekly"
            });

            // 5. 学习之星：本周获得星星达到 10 颗
            var weekTotalEarned = await query
                .Where(r => !r.IsDeleted && r.Date >= startOfWeek && r.Date <= today && r.StarCount > 0)
                .SumAsync(r => r.StarCount);
            tasks.Add(new DashboardTaskDto
            {
                Id = 5,
                Name = "学习之星",
                Description = "本周通过好表现获得 10 颗星星",
                Icon = "⭐",
                CurrentProgress = Math.Min(weekTotalEarned, 10),
                TargetProgress = 10,
                BonusStars = 1,
                IsCompleted = weekTotalEarned >= 10,
                Type = "Weekly"
            });

            return Ok(APIResponse<List<DashboardTaskDto>>.Ok(tasks));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务列表失败");
            return StatusCode(500, APIResponse<List<DashboardTaskDto>>.Fail("获取任务失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 获取合成台物品列表
    /// </summary>
    [HttpGet("crafting")]
    [ProducesResponseType(typeof(APIResponse<List<DashboardCraftingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<DashboardCraftingDto>>>> GetCraftingItems(
        [FromQuery] string? userId = null)
    {
        try
        {
            // 获取当前余额
            var query = _context.StarRecords.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            var balance = await query
                .Where(r => !r.IsDeleted)
                .SumAsync(r => r.StarCount);

            // 从 ReasonTemplateService 获取花费模板
            var spendTemplates = await _reasonTemplateService.GetSpendTemplatesAsync();

            var items = new List<DashboardCraftingDto>();
            var index = 1;

            foreach (var template in spendTemplates)
            {
                // 解析星星数量（从 Key 中提取，或使用 Value）
                var cost = Math.Abs(template.Value);

                items.Add(new DashboardCraftingDto
                {
                    Id = index++,
                    Name = template.Key,
                    Icon = GetIconForReason(template.Key),
                    Cost = cost,
                    Available = balance >= cost,
                    Status = balance >= cost ? "craftable" : "insufficient"
                });
            }

            return Ok(APIResponse<List<DashboardCraftingDto>>.Ok(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取合成台数据失败");
            return StatusCode(500, APIResponse<List<DashboardCraftingDto>>.Fail("获取数据失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 获取星轨日志（最近动态）
    /// </summary>
    [HttpGet("activities")]
    [ProducesResponseType(typeof(APIResponse<PagedResult<DashboardActivityDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<PagedResult<DashboardActivityDto>>>> GetActivities(
        [FromQuery] string? type,
        [FromQuery] string? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            pageSize = Math.Min(pageSize, 50);
            page = Math.Max(page, 1);

            var query = _context.StarRecords
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "reward")
                    query = query.Where(r => r.StarCount > 0);
                else if (type == "spend")
                    query = query.Where(r => r.StarCount < 0);
            }

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new DashboardActivityDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    StarCount = r.StarCount,
                    Reason = r.Reason,
                    Type = r.Type,
                    ImagePath = r.ImagePath,
                    ImageFileName = r.ImageFileName
                })
                .ToListAsync();

            var result = new PagedResult<DashboardActivityDto>
            {
                Items = records,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(APIResponse<PagedResult<DashboardActivityDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取活动日志失败");
            return StatusCode(500, APIResponse<PagedResult<DashboardActivityDto>>.Fail("获取日志失败：" + ex.Message));
        }
    }

    #region Private Methods

    private string GetIconForReason(string reason)
    {
        return reason switch
        {
            var r when r.Contains("游戏") => "🎮",
            var r when r.Contains("玩具") => "🧸",
            var r when r.Contains("零食") => "🍭",
            var r when r.Contains("乐高") => "🧱",
            var r when r.Contains("书") => "📖",
            _ => "🎁"
        };
    }

    #endregion
}
