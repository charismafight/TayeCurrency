using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;

namespace Taye.WebAPI.Services;

public class AchievementService : IAchievementService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(AppDbContext context, ILogger<AchievementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AchievementDto>> GetPlayerAchievementsAsync(string? userId = null)
    {
        var effectiveUserId = userId ?? "Taye";
        var definitions = await _context.AchievementDefinitions
            .Where(a => a.IsActive)
            .OrderBy(a => a.SortOrder)
            .ToListAsync();

        var playerAchievements = await _context.PlayerAchievements
            .Where(p => p.UserId == effectiveUserId)
            .ToDictionaryAsync(p => p.AchievementId, p => p);

        var result = new List<AchievementDto>();

        foreach (var def in definitions)
        {
            var milestones = JsonSerializer.Deserialize<List<MilestoneDto>>(def.MilestonesJson) ?? new();
            var player = playerAchievements.GetValueOrDefault(def.AchievementId);

            var currentCount = player?.CurrentCount ?? 0;
            var unlockedIndex = player?.LastMilestoneIndex ?? -1;

            var unlockedMilestones = milestones
                .Where((m, i) => i <= unlockedIndex)
                .ToList();

            var nextMilestone = unlockedIndex + 1 < milestones.Count
                ? milestones[unlockedIndex + 1]
                : null;

            result.Add(new AchievementDto
            {
                Id = def.AchievementId,
                Name = def.Name,
                Icon = def.Icon,
                Category = def.Category,
                CurrentCount = currentCount,
                Milestones = milestones,
                UnlockedMilestoneIndex = unlockedIndex,
                IsUnlocked = unlockedIndex >= 0,
                IsHidden = def.IsHidden,
                NextMilestoneTitle = nextMilestone?.Title,
                NextMilestoneCount = nextMilestone?.Count
            });
        }

        // 隐藏成就不返回
        return result.Where(a => !a.IsHidden).ToList();
    }

    public async Task CheckAndUnlockAchievementsAsync(string reason, string? userId = null)
    {
        var effectiveUserId = userId ?? "Taye";
        var definitions = await _context.AchievementDefinitions
            .Where(a => a.IsActive && !a.IsHidden)
            .ToListAsync();

        foreach (var def in definitions)
        {
            if (!reason.Contains(def.MatchReason)) continue;

            var player = await _context.PlayerAchievements
                .FirstOrDefaultAsync(p => p.AchievementId == def.AchievementId && p.UserId == effectiveUserId);

            if (player == null)
            {
                player = new PlayerAchievement
                {
                    AchievementId = def.AchievementId,
                    Name = def.Name,
                    CurrentCount = 0,
                    LastMilestoneIndex = -1,
                    UserId = effectiveUserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PlayerAchievements.Add(player);
            }

            player.CurrentCount++;
            player.UpdatedAt = DateTime.UtcNow;

            var milestones = JsonSerializer.Deserialize<List<MilestoneDto>>(def.MilestonesJson) ?? new();
            var nextIndex = player.LastMilestoneIndex + 1;

            if (nextIndex < milestones.Count && player.CurrentCount >= milestones[nextIndex].Count)
            {
                // 解锁里程碑
                player.LastMilestoneIndex = nextIndex;
                var milestone = milestones[nextIndex];

                // 发放奖励
                var bonusRecord = new StarRecord
                {
                    Date = DateTime.Now,
                    StarCount = milestone.Bonus,
                    Reason = $"🎉 解锁成就: {def.Name} - {milestone.Title}",
                    Type = "Reward",
                    Notes = "成就奖励",
                    UserId = effectiveUserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StarRecords.Add(bonusRecord);
                _logger.LogInformation("解锁成就: {Name} - {Title}", def.Name, milestone.Title);
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task CheckHiddenAchievementsAsync(string? userId = null)
    {
        var effectiveUserId = userId ?? "Taye";
        var hiddenDefs = await _context.AchievementDefinitions
            .Where(a => a.IsActive && a.IsHidden)
            .ToListAsync();

        foreach (var def in hiddenDefs)
        {
            var alreadyUnlocked = await _context.PlayerAchievements
                .AnyAsync(p => p.AchievementId == def.AchievementId
                    && p.UserId == effectiveUserId
                    && p.LastMilestoneIndex >= 0);

            if (alreadyUnlocked) continue;

            var triggerConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(def.TriggerConfig ?? "{}");
            var isTriggered = await EvaluateHiddenTrigger(def.AchievementId, triggerConfig, effectiveUserId);

            if (isTriggered)
            {
                var milestones = JsonSerializer.Deserialize<List<MilestoneDto>>(def.MilestonesJson) ?? new();
                if (milestones.Count == 0) continue;

                var player = new PlayerAchievement
                {
                    AchievementId = def.AchievementId,
                    Name = def.Name,
                    CurrentCount = 1,
                    LastMilestoneIndex = 0,
                    UserId = effectiveUserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PlayerAchievements.Add(player);

                var bonusRecord = new StarRecord
                {
                    Date = DateTime.Now,
                    StarCount = milestones[0].Bonus,
                    Reason = $"🎉 解锁隐藏成就: {def.Name}",
                    Type = "Reward",
                    Notes = "隐藏成就奖励",
                    UserId = effectiveUserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StarRecords.Add(bonusRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation("解锁隐藏成就: {Name}", def.Name);
            }
        }
    }

    private async Task<bool> EvaluateHiddenTrigger(string achievementId, Dictionary<string, object>? config, string userId)
    {
        if (config == null) return false;

        return achievementId switch
        {
            "early_bird" => await CheckEarlyBird(userId),
            "bookworm" => await CheckBookworm(userId),
            "self_discipline" => await CheckSelfDiscipline(userId),
            "all_around" => await CheckAllAround(userId),
            _ => false
        };
    }

    private async Task<bool> CheckEarlyBird(string userId)
    {
        // 连续 3 天 21:30 前上床
        var today = DateTime.Today;
        var records = await _context.StarRecords
            .Where(r => r.UserId == userId
                && !r.IsDeleted
                && r.Date >= today.AddDays(-3)
                && r.Reason.Contains("21:30前"))
            .GroupBy(r => r.Date.Date)
            .Select(g => g.Count())
            .ToListAsync();

        return records.Count == 3 && records.All(c => c >= 1);
    }

    private async Task<bool> CheckBookworm(string userId)
    {
        var count = await _context.StarRecords
            .Where(r => r.UserId == userId
                && !r.IsDeleted
                && r.Reason.Contains("雇佣爸爸读书"))
            .CountAsync();
        return count >= 5;
    }

    private async Task<bool> CheckSelfDiscipline(string userId)
    {
        // 本周每日任务全部完成 ≥ 5 天
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
        var completions = await _context.TaskCompletions
            .Where(t => t.UserId == userId
                && t.PeriodType == "Daily"
                && t.PeriodKey.CompareTo(startOfWeek.ToString("yyyy-MM-dd")) >= 0
                && t.IsCompleted)
            .CountAsync();
        return completions >= 5;
    }

    private async Task<bool> CheckAllAround(string userId)
    {
        var count = await _context.PlayerAchievements
            .Where(p => p.UserId == userId && p.LastMilestoneIndex >= 0)
            .CountAsync();
        return count >= 3;
    }
}
