using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;
using Taye.Shared.DTOs;

namespace Taye.MAUI.Services;


public interface IApiService
{
    Task<List<StarRecordDto>> GetRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null);
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

    public ApiService(IConfiguration config, ILogger<ApiService> logger)
    {
        _logger = logger;
        _baseUrl = config["ApiBaseUrl"] ?? "https://localhost:5001";

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _api = RestService.For<IStarRecordApi>(httpClient);
    }

    public async Task<List<StarRecordDto>> GetRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, string? type = null)
    {
        try
        {
            var response = await _api.GetRecords(startDate, endDate, type);
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
            var response = await _api.GetStatistics();
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
