using Taye.Shared.DTOs;

namespace Taye.WebAPI.Services;

public interface ITaskService
{
    /// <summary>
    /// 获取今日任务列表（含完成状态）
    /// </summary>
    Task<TasksResponseDto> GetTodayTasksAsync(string? userId = null);

    /// <summary>
    /// 获取指定日期的任务完成情况
    /// </summary>
    Task<TasksResponseDto> GetTasksByDateAsync(DateTime date, string? userId = null);

    /// <summary>
    /// 手动刷新今日任务状态（重新计算）
    /// </summary>
    Task RefreshTodayTasksAsync(string? userId = null);
}
