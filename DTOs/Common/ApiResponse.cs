namespace ToDoList.Api.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> Fail(string code, string message, Dictionary<string, string[]>? details = null) => new()
    {
        Success = false,
        Error = new ApiError { Code = code, Message = message, Details = details }
    };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse OkEmpty() => new() { Success = true };
}