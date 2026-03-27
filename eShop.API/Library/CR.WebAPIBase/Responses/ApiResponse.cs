// Library/CR.WebAPIBase/Responses/ApiResponse.cs
namespace CR.WebAPIBase.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(T data, string? msg = null) => new()
    {
        Success = true,
        Data = data,
        StatusCode = 200,
        Message = msg
    };

    public static ApiResponse<T> Fail(string error, int code = 400) => new()
    {
        Success = false,
        Errors = new List<string> { error },
        StatusCode = code
    };
}