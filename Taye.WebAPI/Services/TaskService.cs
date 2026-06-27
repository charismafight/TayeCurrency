using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;

namespace Taye.WebAPI.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;

    private readonly List<TaskDefinition> _taskDefinitions = new()
{
    new TaskDefinition
    {
        Id = "bedtime",
        Name = "按时上床睡觉",
        Icon = "🌙",
        Target = 1,
        DefaultCount = 0,
        ReasonMatches = new[] { "晚上21:30前收好书包，洗漱完毕，换好睡衣，上自己的床" },
        PunishMatches = Array.Empty<string>()
    },
    new TaskDefinition
    {
        Id = "clean_plate",
        Name = "光盘行动",
        Icon = "🍽️",
        Target = 2,
        DefaultCount = 0,
        ReasonMatches = new[] { "吃饭光盘" },
        PunishMatches = Array.Empty<string>()
    },
    new TaskDefinition
    {
        Id = "homework",
        Name = "主动完成作业",
        Icon = "📋",
        Target = 1,
        DefaultCount = 0,
        ReasonMatches = new[] { "主动完成常规作业" },
        PunishMatches = Array.Empty<string>()
    },
    new TaskDefinition
    {
        Id = "tidy_table",
        Name = "整理餐桌",
        Icon = "🧹",
        Target = 1,
        DefaultCount = 0,
        ReasonMatches = new[] { "吃完饭清理桌面并收碗" },
        PunishMatches = Array.Empty<string>()
    },
    new TaskDefinition
    {
        Id = "brush_teeth",
        Name = "洗脸刷牙",
        Icon = "🪥",
        Target = 2,
        DefaultCount = 2,  // ✅ 默认完成 2 次
        ReasonMatches = Array.Empty<string>(),
        PunishMatches = new[] { "忘记洗脸/忘记刷牙" }
    },
};

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TasksResponseDto> GetTodayTasksAsync(string? userId = null)
    {
        var today = DateTime.Today;
        return await GetTasksByDateInternalAsync(today, userId);
    }

    public async Task<TasksResponseDto> GetTasksByDateAsync(DateTime date, string? userId = null)
    {
        return await GetTasksByDateInternalAsync(date, userId);
    }

    private async Task<TasksResponseDto> GetTasksByDateInternalAsync(DateTime date, string? userId = null)
    {
        var periodKey = date.ToString("yyyy-MM-dd");
        var tasks = new List<TaskDto>();
        var allCompleted = true;

        foreach (var def in _taskDefinitions)
        {
            // 查询今日该任务的完成次数
            var completedCount = await _context.StarRecords
                .Where(r => !r.IsDeleted
                    && r.Date.Date == date
                    && r.StarCount > 0
                    && def.ReasonMatches.Any(m => r.Reason.Contains(m)))
                .CountAsync();

            var isCompleted = completedCount >= def.Target;
            if (!isCompleted) allCompleted = false;

            tasks.Add(new TaskDto
            {
                Id = def.Id,
                Name = def.Name,
                Icon = def.Icon,
                Target = def.Target,
                Current = Math.Min(completedCount, def.Target),
                IsCompleted = isCompleted
            });
        }

        // 检查奖励是否已领取
        var completion = await _context.TaskCompletions
            .FirstOrDefaultAsync(t => t.PeriodKey == periodKey && t.UserId == userId);

        var bonusEarned = completion?.BonusEarned ?? false;

        // 如果全部完成且未领取，自动发放奖励并记录
        if (allCompleted && !bonusEarned)
        {
            // 发放奖励（插入一条 StarRecord）
            var bonusRecord = new StarRecord
            {
                Date = DateTime.Now,
                StarCount = 1,
                Reason = "🎉 完成全部每日任务",
                Type = "Reward",
                Notes = "每日任务奖励",
                CreatedAt = DateTime.UtcNow
            };
            _context.StarRecords.Add(bonusRecord);
            await _context.SaveChangesAsync();

            // 更新或创建完成记录
            if (completion == null)
            {
                completion = new TaskCompletion
                {
                    TaskId = "daily_all",
                    TaskName = "全部每日任务",
                    PeriodType = "Daily",
                    PeriodKey = periodKey,
                    TargetCount = _taskDefinitions.Count,
                    CompletedCount = _taskDefinitions.Count,
                    IsCompleted = true,
                    BonusEarned = true,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.TaskCompletions.Add(completion);
            }
            else
            {
                completion.BonusEarned = true;
                completion.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            bonusEarned = true;
        }

        // 如果全部完成但已领取，确保记录存在
        if (allCompleted && completion == null)
        {
            completion = new TaskCompletion
            {
                TaskId = "daily_all",
                TaskName = "全部每日任务",
                PeriodType = "Daily",
                PeriodKey = periodKey,
                TargetCount = _taskDefinitions.Count,
                CompletedCount = _taskDefinitions.Count,
                IsCompleted = true,
                BonusEarned = true,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.TaskCompletions.Add(completion);
            await _context.SaveChangesAsync();
        }

        return new TasksResponseDto
        {
            Date = periodKey,
            Tasks = tasks,
            AllCompleted = allCompleted,
            BonusStars = 1,
            BonusEarned = bonusEarned
        };
    }

    public async Task RefreshTodayTasksAsync(string? userId = null)
    {
        var today = DateTime.Today;
        var periodKey = today.ToString("yyyy-MM-dd");

        // 删除今日的完成记录（重新计算）
        var existing = await _context.TaskCompletions
            .Where(t => t.PeriodKey == periodKey && t.UserId == userId)
            .ToListAsync();

        if (existing.Any())
        {
            _context.TaskCompletions.RemoveRange(existing);
            await _context.SaveChangesAsync();
        }

        // 重新计算
        await GetTasksByDateInternalAsync(today, userId);
    }

    private class TaskDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Target { get; set; }

        // 新增：默认完成次数（针对有惩罚无奖励的任务）
        public int DefaultCount { get; set; } = 0;

        // 新增：正向匹配关键词（奖励记录）
        public string[] ReasonMatches { get; set; } = Array.Empty<string>();

        // 新增：惩罚匹配关键词（惩罚记录）
        public string[] PunishMatches { get; set; } = Array.Empty<string>();
    }
}
