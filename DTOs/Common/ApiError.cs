namespace ToDoList.Api.DTOs.Common;

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string[]>? Details { get; set; }
}