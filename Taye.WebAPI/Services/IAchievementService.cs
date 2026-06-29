using Taye.Shared.DTOs;

namespace Taye.WebAPI.Services;

public interface IAchievementService
{
    Task<List<AchievementDto>> GetPlayerAchievementsAsync(string? userId = null);
    Task CheckAndUnlockAchievementsAsync(string reason, string? userId = null);
    Task CheckHiddenAchievementsAsync(string? userId = null);
}
