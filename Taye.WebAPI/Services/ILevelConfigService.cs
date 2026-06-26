using System.Collections.Generic;
using System.Threading.Tasks;
using Taye.Shared.DTOs;

namespace Taye.WebAPI.Services;

/// <summary>
/// 等级配置服务接口
/// </summary>
public interface ILevelConfigService
{
    /// <summary>
    /// 获取所有等级配置（按 RequiredStars 升序）
    /// </summary>
    Task<List<LevelConfigDto>> GetAllLevelsAsync();

    /// <summary>
    /// 根据星星总数计算当前等级和下一等级
    /// </summary>
    Task<(string currentRank, string nextRank, int expPercent)> CalculateLevelAsync(int totalStars);

    /// <summary>
    /// 获取单个等级配置
    /// </summary>
    Task<LevelConfigDto?> GetLevelByIdAsync(int id);

    /// <summary>
    /// 创建等级配置
    /// </summary>
    Task<LevelConfigDto> CreateLevelAsync(LevelConfigDto dto);

    /// <summary>
    /// 更新等级配置
    /// </summary>
    Task<LevelConfigDto?> UpdateLevelAsync(int id, LevelConfigDto dto);

    /// <summary>
    /// 删除等级配置（软删除）
    /// </summary>
    Task<bool> DeleteLevelAsync(int id);
}
