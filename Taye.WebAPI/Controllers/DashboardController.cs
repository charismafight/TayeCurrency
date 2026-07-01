using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;
using Taye.WebAPI.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using Taye.Shared;

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
    private readonly ITaskService _taskService;

    public DashboardController(
        AppDbContext context,
        ILogger<DashboardController> logger,
        IReasonTemplateService reasonTemplateService,
        ILevelConfigService levelConfigService,
        ITaskService taskService)
    {
        _context = context;
        _logger = logger;
        _reasonTemplateService = reasonTemplateService;
        _levelConfigService = levelConfigService;
        _taskService = taskService;
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
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
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
            var yesterdayBalance = await query
                .Where(r => !r.IsDeleted && r.Date.UtcDateTime < today)
                .SumAsync(r => r.StarCount);

            // 本周获得
            var weeklyEarned = await query
                .Where(r => !r.IsDeleted && r.Date.UtcDateTime >= startOfWeek && r.StarCount > 0)
                .SumAsync(r => r.StarCount);

            // 本周消费（取绝对值）
            var weeklySpent = await query
                .Where(r => !r.IsDeleted && r.Date.UtcDateTime >= startOfWeek && r.StarCount < 0)
                .SumAsync(r => -r.StarCount);

            var weeklyPunished = await query
                .Where(r => !r.IsDeleted && r.Date.UtcDateTime >= startOfWeek && r.Type == "Punish")
                .SumAsync(r => Math.Abs(r.StarCount));

            // 计算等级
            var (rank, nextRank, expPercent) = await _levelConfigService.CalculateLevelAsync(totalStars);

            var dto = new DashboardProfileDto
            {
                PlayerName = Constants.DefaultUserName,
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
        [FromQuery] string? userId = "Taye",
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

    [HttpGet("tasks")]
    [ProducesResponseType(typeof(APIResponse<TasksResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<TasksResponseDto>>> GetTasks([FromQuery] string? userId = null)
    {
        try
        {
            var result = await _taskService.GetTodayTasksAsync(userId);
            return Ok(APIResponse<TasksResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取今日任务失败");
            return StatusCode(500, APIResponse<TasksResponseDto>.Fail("获取任务失败：" + ex.Message));
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
