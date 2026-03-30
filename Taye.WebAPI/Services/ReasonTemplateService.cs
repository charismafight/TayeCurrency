using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.WebAPI.Data;

namespace Taye.WebAPI.Services;

public interface IReasonTemplateService
{
    Task<Dictionary<string, int>> GetRewardTemplatesAsync();   // 奖励类
    Task<Dictionary<string, int>> GetSpendTemplatesAsync();    // 花费类
    Task<Dictionary<string, int>> GetPunishTemplatesAsync();   // 惩罚类
    Task<Dictionary<string, int>> GetTemplatesByTypeAsync(string type);
    Task<List<ReasonTemplate>> GetAllTemplatesAsync();
    Task<ReasonTemplate?> GetTemplateByIdAsync(int id);
    Task<bool> AddTemplateAsync(ReasonTemplate template);
    Task<bool> UpdateTemplateAsync(ReasonTemplate template);
    Task<bool> DeleteTemplateAsync(int id);
}

public class ReasonTemplateService : IReasonTemplateService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReasonTemplateService> _logger;

    public ReasonTemplateService(AppDbContext context, ILogger<ReasonTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Dictionary<string, int>> GetRewardTemplatesAsync()
    {
        return await _context.ReasonTemplates
            .Where(t => t.Type == "Reward" && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToDictionaryAsync(t => t.Reason, t => t.StarCount);
    }

    public async Task<Dictionary<string, int>> GetSpendTemplatesAsync()
    {
        return await _context.ReasonTemplates
            .Where(t => t.Type == "Spend" && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToDictionaryAsync(t => t.Reason, t => t.StarCount);
    }

    public async Task<Dictionary<string, int>> GetPunishTemplatesAsync()
    {
        return await _context.ReasonTemplates
            .Where(t => t.Type == "Punish" && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToDictionaryAsync(t => t.Reason, t => t.StarCount);
    }

    public async Task<Dictionary<string, int>> GetTemplatesByTypeAsync(string type)
    {
        var typeEn = type switch
        {
            "奖励" => "Reward",
            "花费" => "Spend",
            "惩罚" => "Punish",
            _ => type
        };

        return await _context.ReasonTemplates
            .Where(t => t.Type == typeEn && t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToDictionaryAsync(t => t.Reason, t => t.StarCount);
    }

    public async Task<List<ReasonTemplate>> GetAllTemplatesAsync()
    {
        return await _context.ReasonTemplates
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<ReasonTemplate?> GetTemplateByIdAsync(int id)
    {
        return await _context.ReasonTemplates.FindAsync(id);
    }

    public async Task<bool> AddTemplateAsync(ReasonTemplate template)
    {
        try
        {
            template.CreatedAt = DateTime.UtcNow;
            _context.ReasonTemplates.Add(template);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加模板失败");
            return false;
        }
    }

    public async Task<bool> UpdateTemplateAsync(ReasonTemplate template)
    {
        try
        {
            var existing = await _context.ReasonTemplates.FindAsync(template.Id);
            if (existing == null) return false;

            existing.Reason = template.Reason;
            existing.StarCount = template.StarCount;
            existing.Type = template.Type;
            existing.Notes = template.Notes;
            existing.SortOrder = template.SortOrder;
            existing.IsActive = template.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新模板失败");
            return false;
        }
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        try
        {
            var template = await _context.ReasonTemplates.FindAsync(id);
            if (template == null) return false;

            // 软删除
            template.IsActive = false;
            template.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除模板失败");
            return false;
        }
    }
}
