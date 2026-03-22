using Refit;
using Taye.Shared.DTOs;

namespace Taye.MAUI.Services;

public interface IStarRecordApi
{
    /// <summary>
    /// 获取所有星星记录
    /// </summary>
    [Get("/api/StarRecords")]
    Task<APIResponse<List<StarRecordDto>>> GetRecords(
        [Query] DateTime? startDate = null,
        [Query] DateTime? endDate = null,
        [Query] string? type = null,
        [Query] string? userId = null);

    /// <summary>
    /// 根据ID获取单条记录
    /// </summary>
    [Get("/api/StarRecords/{id}")]
    Task<APIResponse<StarRecordDto>> GetRecord(int id);

    /// <summary>
    /// 创建记录（带图片上传）
    /// </summary>
    [Multipart]
    [Post("/api/StarRecords")]
    Task<APIResponse<StarRecordDto>> CreateRecord(
        [AliasAs("Date")] string date,
        [AliasAs("StarCount")] int starCount,
        [AliasAs("Reason")] string reason,
        [AliasAs("Type")] string type,
        [AliasAs("Notes")] string? notes,
        [AliasAs("ImageFile")] StreamPart? imageFile);

    /// <summary>
    /// 更新记录
    /// </summary>
    [Multipart]
    [Put("/api/StarRecords/{id}")]
    Task<APIResponse<StarRecordDto>> UpdateRecord(
        int id,
        [AliasAs("Date")] string date,
        [AliasAs("StarCount")] int starCount,
        [AliasAs("Reason")] string reason,
        [AliasAs("Type")] string type,
        [AliasAs("Notes")] string? notes,
        [AliasAs("ImageFile")] StreamPart? imageFile);

    /// <summary>
    /// 删除记录
    /// </summary>
    [Delete("/api/StarRecords/{id}")]
    Task<APIResponse<bool>> DeleteRecord(int id);

    /// <summary>
    /// 物理删除记录
    /// </summary>
    [Delete("/api/StarRecords/{id}/hard")]
    Task<APIResponse<bool>> HardDeleteRecord(int id);

    /// <summary>
    /// 获取统计数据
    /// </summary>
    [Get("/api/StarRecords/statistics")]
    Task<APIResponse<StarStatisticsDto>> GetStatistics([Query] string? userId = null);
}
