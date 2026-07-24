namespace BasicArchitecture.Core.Result;

public enum ResultTypeEnum
{
    None = 0,
    Information = 1,
    Success = 2,
    Warning = 3,
    Error = 4,
    Redirect = 5,
    Nothing = 404,
    NotFound = 6,
    Activation = 7
}

public interface IResult
{
    bool IsSuccess { get; set; }
    string Message { get; set; }
    ResultTypeEnum ResultType { get; set; }
}

public interface IResult<T> : IResult
{
    T Data { get; set; }
}

public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; set; }
    public ResultTypeEnum ResultType { get; set; }
    public string Html { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsLastPackage { get; set; }
    public T Data { get; set; }
    public int ResultCount { get; set; }
    public int PageCount { get; set; }
    public string[] MessagesParameters { get; set; } = Array.Empty<string>();

    public Result()
    {
        Data = default!;
    }

    public Result(bool isSuccess, ResultTypeEnum resultType, T data, string message)
    {
        IsSuccess = isSuccess;
        ResultType = resultType;
        Data = data;
        Message = message;
    }

    public Result(bool isSuccess, ResultTypeEnum resultType, T data, int pageCount, int resultCount, string message)
    {
        IsSuccess = isSuccess;
        ResultType = resultType;
        Data = data;
        PageCount = pageCount;
        ResultCount = resultCount;
        Message = message;
    }
}
