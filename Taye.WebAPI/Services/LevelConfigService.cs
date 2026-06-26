using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;

namespace Taye.WebAPI.Services;

/// <summary>
/// 等级配置服务实现
/// </summary>
public class LevelConfigService : ILevelConfigService
{
    private readonly AppDbContext _context;

    public LevelConfigService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LevelConfigDto>> GetAllLevelsAsync()
    {
        return await _context.LevelConfigs
            .Where(l => l.IsActive)
            .OrderBy(l => l.RequiredStars)
            .Select(l => new LevelConfigDto
            {
                Id = l.Id,
                LevelNumber = l.LevelNumber,
                LevelName = l.LevelName,
                LevelIcon = l.LevelIcon,
                RequiredStars = l.RequiredStars,
                IsActive = l.IsActive
            })
            .ToListAsync();
    }

    public async Task<(string currentRank, string nextRank, int expPercent)> CalculateLevelAsync(int totalStars)
    {
        var levels = await _context.LevelConfigs
            .Where(l => l.IsActive)
            .OrderBy(l => l.RequiredStars)
            .ToListAsync();

        if (!levels.Any())
        {
            return ("🌱 星尘学徒", "🔥 流星勇士", 0);
        }

        LevelConfig? currentLevel = null;
        LevelConfig? nextLevel = null;

        foreach (var level in levels)
        {
            if (totalStars >= level.RequiredStars)
            {
                currentLevel = level;
            }
            else
            {
                nextLevel = level;
                break;
            }
        }

        var currentName = currentLevel != null
            ? $"{currentLevel.LevelIcon}{currentLevel.LevelName}"
            : "🌱 星尘学徒";

        if (nextLevel == null)
        {
            return (currentName, "🏆 满级", 100);
        }

        var expPercent = (int)((totalStars - currentLevel!.RequiredStars) / (double)(nextLevel.RequiredStars - currentLevel.RequiredStars) * 100);
        expPercent = Math.Min(100, Math.Max(0, expPercent));

        var nextName = $"{nextLevel.LevelIcon}{nextLevel.LevelName}";

        return (currentName, nextName, expPercent);
    }

    public async Task<LevelConfigDto?> GetLevelByIdAsync(int id)
    {
        var level = await _context.LevelConfigs.FindAsync(id);
        if (level == null) return null;

        return new LevelConfigDto
        {
            Id = level.Id,
            LevelNumber = level.LevelNumber,
            LevelName = level.LevelName,
            LevelIcon = level.LevelIcon,
            RequiredStars = level.RequiredStars,
            IsActive = level.IsActive
        };
    }

    public async Task<LevelConfigDto> CreateLevelAsync(LevelConfigDto dto)
    {
        var entity = new LevelConfig
        {
            LevelNumber = dto.LevelNumber,
            LevelName = dto.LevelName,
            LevelIcon = dto.LevelIcon,
            RequiredStars = dto.RequiredStars,
            IsActive = dto.IsActive,
            SortOrder = dto.LevelNumber
        };

        _context.LevelConfigs.Add(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        return dto;
    }

    public async Task<LevelConfigDto?> UpdateLevelAsync(int id, LevelConfigDto dto)
    {
        var entity = await _context.LevelConfigs.FindAsync(id);
        if (entity == null) return null;

        entity.LevelNumber = dto.LevelNumber;
        entity.LevelName = dto.LevelName;
        entity.LevelIcon = dto.LevelIcon;
        entity.RequiredStars = dto.RequiredStars;
        entity.IsActive = dto.IsActive;
        entity.SortOrder = dto.LevelNumber;

        await _context.SaveChangesAsync();

        return dto;
    }

    public async Task<bool> DeleteLevelAsync(int id)
    {
        var entity = await _context.LevelConfigs.FindAsync(id);
        if (entity == null) return false;

        entity.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}
