namespace CR.DtoBase;

/// <summary>
/// Kết quả trả về từ một hàm void
/// </summary>
public class Result
{
    private static readonly Result SuccessResult = new();

    public bool IsSuccess { get; protected set; }
    public bool IsFailure { get => !IsSuccess; }
    public int ErrorCode { get; protected set; }
    public object? OtherData { get; protected set; }
    public string[]? ListParam { get; protected set; }
    public string StackTrace { get; protected set; } = null!;

    public Result()
    {
        IsSuccess = true;
        ErrorCode = 0;
    }

    /// <summary>
    /// Constructor cho kết quả thất bại
    /// </summary>
    protected Result(Result result)
    {
        IsSuccess = false;
        OtherData = result.OtherData;
        ErrorCode = result.ErrorCode;
        StackTrace = result.StackTrace;
        ListParam = result.ListParam;
    }

    /// <summary>
    /// Constructor cho kết quả thất bại đầu tiên trong stack trace
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    protected Result(int errorCode, string stackTrace)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        StackTrace = stackTrace;
    }

    /// <summary>
    /// Constructor cho kết quả thất bại đầu tiên trong stack trace
    /// nhận thêm data trả ra
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    protected Result(int errorCode, string stackTrace, object otherData)
        : this(errorCode, stackTrace)
    {
        OtherData = otherData;
    }

    /// <summary>
    /// Constructor cho kết quả thất bại đầu tiên trong stack trace
    /// nhận thêm data trả ra và list param phục vụ cho localization
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <param name="listParam"></param>
    protected Result(int errorCode, string stackTrace, object otherData, params string[] listParam)
        : this(errorCode, stackTrace, otherData)
    {
        ListParam = listParam;
    }

    /// <summary>
    /// Constructor cho kết quả thất bại đầu tiên trong stack trace
    /// nhận thêm list param phục vụ cho localization
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="listParam"></param>
    protected Result(int errorCode, string stackTrace, params string[] listParam)
        : this(errorCode, stackTrace)
    {
        ListParam = listParam;
    }

    /// <summary>
    /// Factory method cho thành công
    /// </summary>
    /// <returns></returns>
    public static Result Success()
    {
        return SuccessResult;
    }

    /// <summary>
    /// Forward sang hàm Result&lt;T&gt;.Success(value)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Success<T>(T value)
    {
        return Result<T>.Success(value);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <returns></returns>
    public static Result Failure(int errorCode, string stackTrace)
    {
        return new Result(errorCode, stackTrace);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <returns></returns>
    public static Result Failure(int errorCode, string stackTrace, object otherData)
    {
        return new Result(errorCode, stackTrace, otherData);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="listParam"></param>
    /// <returns></returns>
    public static Result Failure(int errorCode, string stackTrace, params string[] listParam)
    {
        return new Result(errorCode, stackTrace, listParam: listParam);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <param name="listParam"></param>
    /// <returns></returns>
    public static Result Failure(
        int errorCode,
        string stackTrace,
        object otherData,
        params string[] listParam
    )
    {
        return new Result(errorCode, stackTrace, otherData, listParam);
    }

    /// <summary>
    /// Method để trả ra bên ngoài nối tiếp chuỗi gọi stack trace
    /// </summary>
    public static Result Failure(Result result)
    {
        return new Result(result);
    }

    /// <summary>
    /// Forward sang hàm Result&lt;T&gt;.Failure(errorCode, stackTrace)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <returns></returns>
    public static Result<T> Failure<T>(int errorCode, string stackTrace)
    {
        return Result<T>.Failure(errorCode, stackTrace);
    }

    /// <summary>
    /// Forward sang hàm Result&lt;T&gt;.Failure(result)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Result<T> Failure<T>(Result result)
    {
        return Result<T>.Failure(result);
    }

    /// <summary>
    /// Forward sang hàm Result&lt;T&gt;.Failure(errorCode, stackTrace, otherData: otherData)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Result<T> Failure<T>(int errorCode, string stackTrace, object otherData)
    {
        return Result<T>.Failure(errorCode, stackTrace, otherData: otherData);
    }

    /// <summary>
    /// Forward sang hàm Result&lt;T&gt;.Failure(errorCode, stackTrace, listParam: listParam)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Result<T> Failure<T>(int errorCode, string stackTrace, params string[] listParam)
    {
        return Result<T>.Failure(errorCode, stackTrace, listParam: listParam);
    }
}

public class Result<T> : Result
{
    public T Value { get; private set; }

    /// <summary>
    /// Constructor cho kết quả thành công
    /// </summary>
    /// <param name="value"></param>
    private Result(T value)
        : base()
    {
        Value = value;
    }

    protected Result(int errorCode, string stackTrace)
        : base(errorCode, stackTrace)
    {
        Value = default!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    private Result(Result result)
        : base(result)
    {
        Value = default!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    public Result(int errorCode, string stackTrace, object otherData)
        : base(errorCode, stackTrace, otherData)
    {
        Value = default!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="listParam"></param>
    public Result(int errorCode, string stackTrace, params string[] listParam)
        : base(errorCode, stackTrace, listParam: listParam)
    {
        Value = default!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <param name="listParam"></param>
    public Result(int errorCode, string stackTrace, object otherData, params string[] listParam)
        : base(errorCode, stackTrace, otherData, listParam)
    {
        Value = default!;
    }

    /// <summary>
    /// Factory method cho thành công
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <returns></returns>
    public static new Result<T> Failure(int errorCode, string stackTrace)
    {
        return new Result<T>(errorCode, stackTrace);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <returns></returns>
    public static new Result<T> Failure(int errorCode, string stackTrace, object otherData)
    {
        return new Result<T>(errorCode, stackTrace, otherData);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="listParam"></param>
    /// <returns></returns>
    public static new Result<T> Failure(int errorCode, string stackTrace, params string[] listParam)
    {
        return new Result<T>(errorCode, stackTrace, listParam: listParam);
    }

    /// <summary>
    /// Method để báo lỗi đầu tiên và thêm thông tin khác
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="stackTrace"></param>
    /// <param name="otherData"></param>
    /// <param name="listParam"></param>
    /// <returns></returns>
    public static new Result<T> Failure(
        int errorCode,
        string stackTrace,
        object otherData,
        params string[] listParam
    )
    {
        return new Result<T>(errorCode, stackTrace, otherData, listParam);
    }

    /// <summary>
    /// Method để trả ra bên ngoài nối tiếp chuỗi gọi stack trace
    /// </summary>
    public static new Result<T> Failure(Result result)
    {
        return new Result<T>(result);
    }
}
