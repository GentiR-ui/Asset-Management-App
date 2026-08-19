namespace AssetManagementSystem.Application.Common.Responses;

public class BaseResponse
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }
}

public sealed class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }
}