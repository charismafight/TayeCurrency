namespace Taye.Shared.DTOs;

/// <summary>
/// 分页请求参数
/// </summary>
public class PagedRequest
{
    private int _page = 1;
    private int _pageSize = 20;
    private const int MaxPageSize = 100;

    /// <summary>
    /// 当前页码（从1开始）
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 20 : (value > MaxPageSize ? MaxPageSize : value);
    }
}
