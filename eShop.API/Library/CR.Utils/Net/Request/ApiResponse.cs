namespace CR.Utils.Net.Request
{
    public class ApiResponse
    {
        public StatusCode Status { get; set; }
        public object? Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }

        public ApiResponse(StatusCode status, object? data, int code, string message)
        {
            if (data != null && data.GetType().Name == "Result")
            {
                throw new InvalidOperationException("Error: Object type cannot be Result.");
            }
            Status = status;
            Data = data;
            Code = code;
            Message = message;
        }

        public ApiResponse(object? data)
        {
            if (data != null && data.GetType().Name == "Result")
            {
                throw new InvalidOperationException("Error: Object type cannot be Result.");
            }
            Status = StatusCode.Success;
            Data = data;
            Code = 0;
            Message = "Ok";
        }

        public ApiResponse()
        {
            Status = StatusCode.Success;
            Data = null;
            Code = 0;
            Message = "Ok";
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public new T Data { get; set; }

        public ApiResponse(StatusCode status, T data, int code, string message)
            : base(status, data, code, message)
        {
            Status = status;
            Data = data;
            Code = code;
            Message = message;
        }

        public ApiResponse(T data)
            : base(data)
        {
            Data = data;
        }
    }

    public enum StatusCode
    {
        Success = 1,
        Error = 0
    }
}
