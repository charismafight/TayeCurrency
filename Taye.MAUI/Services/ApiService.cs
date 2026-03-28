using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;
using Taye.Shared.DTOs;

namespace Taye.MAUI.Services;

public interface IApiService
{
    // 原有方法（获取所有记录）
    Task<List<StarRecordDto>> GetRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null);

    // 新增分页方法
    Task<PagedResult<StarRecordDto>> GetRecordsPagedAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null, int page = 1, int pageSize = 20);

    Task<StarRecordDto?> GetRecordAsync(int id);
    Task<bool> CreateRecordAsync(DateTime date, int starCount, string reason, string type, string? notes, Stream? imageStream = null, string? imageFileName = null);
    Task<bool> UpdateRecordAsync(int id, DateTime date, int starCount, string reason, string type, string? notes, Stream? imageStream = null, string? imageFileName = null);
    Task<bool> DeleteRecordAsync(int id);
    Task<StarStatisticsDto?> GetStatisticsAsync();
}

public class ApiService : IApiService
{
    private readonly IStarRecordApi _api;
    private readonly string _baseUrl;
    private readonly ILogger<ApiService> _logger;
    private readonly string? _userId; // 如果需要用户ID

    public ApiService(IConfiguration config, ILogger<ApiService> logger)
    {
        _logger = logger;
        _baseUrl = config["ApiBaseUrl"] ?? "http://localhost:5257";

        // 如果有用户认证，可以从配置或缓存中获取
        _userId = config["UserId"]; // 或者从 SecureStorage 获取

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _api = RestService.For<IStarRecordApi>(httpClient);
    }

    // 原有方法（获取所有记录，不分页）
    public async Task<List<StarRecordDto>> GetRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null)
    {
        try
        {
            var response = await _api.GetRecords(startDate, endDate, type, _userId);
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            _logger.LogWarning("获取记录失败: {Message}", response.Message);
            return new List<StarRecordDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取记录异常");
            return new List<StarRecordDto>();
        }
    }

    // 分页方法 - 由于后端可能不支持分页，使用客户端分页
    public async Task<PagedResult<StarRecordDto>> GetRecordsPagedAsync(
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? type = null,
    int page = 1,
    int pageSize = 20)
    {
        try
        {
            var response = await _api.GetRecordsPaged(startDate, endDate, type, _userId, page, pageSize);
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            _logger.LogWarning("获取分页记录失败: {Message}", response.Message);
            return new PagedResult<StarRecordDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分页记录异常");
            return new PagedResult<StarRecordDto>();
        }
    }

    public async Task<StarRecordDto?> GetRecordAsync(int id)
    {
        try
        {
            var response = await _api.GetRecord(id);
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            _logger.LogWarning("获取单条记录失败: {Message}", response.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取单条记录异常");
            return null;
        }
    }

    public async Task<bool> CreateRecordAsync(DateTime date, int starCount, string reason, string type, string? notes, Stream? imageStream = null, string? imageFileName = null)
    {
        try
        {
            StreamPart? imagePart = null;
            if (imageStream != null && !string.IsNullOrEmpty(imageFileName))
            {
                imagePart = new StreamPart(imageStream, imageFileName, "image/jpeg");
            }

            var response = await _api.CreateRecord(
                date.ToString("yyyy-MM-dd"),
                starCount,
                reason,
                type,
                notes,
                imagePart);

            if (response.Success)
            {
                _logger.LogInformation("创建记录成功");
                return true;
            }

            _logger.LogWarning("创建记录失败: {Message}", response.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建记录异常");
            return false;
        }
    }

    public async Task<bool> UpdateRecordAsync(int id, DateTime date, int starCount, string reason, string type, string? notes, Stream? imageStream = null, string? imageFileName = null)
    {
        try
        {
            StreamPart? imagePart = null;
            if (imageStream != null && !string.IsNullOrEmpty(imageFileName))
            {
                imagePart = new StreamPart(imageStream, imageFileName, "image/jpeg");
            }

            var response = await _api.UpdateRecord(
                id,
                date.ToString("yyyy-MM-dd"),
                starCount,
                reason,
                type,
                notes,
                imagePart);

            if (response.Success)
            {
                _logger.LogInformation("更新记录成功");
                return true;
            }

            _logger.LogWarning("更新记录失败: {Message}", response.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新记录异常");
            return false;
        }
    }

    public async Task<bool> DeleteRecordAsync(int id)
    {
        try
        {
            var response = await _api.DeleteRecord(id);
            if (response.Success)
            {
                _logger.LogInformation("删除记录成功");
                return true;
            }

            _logger.LogWarning("删除记录失败: {Message}", response.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除记录异常");
            return false;
        }
    }

    public async Task<StarStatisticsDto?> GetStatisticsAsync()
    {
        try
        {
            var response = await _api.GetStatistics(_userId);
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            _logger.LogWarning("获取统计失败: {Message}", response.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计异常");
            return null;
        }
    }
}
