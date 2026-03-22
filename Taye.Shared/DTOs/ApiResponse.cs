namespace Taye.Shared.DTOs;

/// <summary>
/// 统一的 API 响应格式
/// </summary>
public class APIResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static APIResponse<T> Ok(T data, string? message = null)
    {
        return new APIResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static APIResponse<T> Fail(string message)
    {
        return new APIResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
