namespace Application.DTOs.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static Result Success(string message = null)
        => new() { IsSuccess = true, Message = message };

    public static Result Error(string message)
        => new() { IsSuccess = false, Message = message };
}

public class Result<T> : Result
{
    public T Data { get; set; }

    public static Result<T> Success(T data, string message = null)
        => new() { IsSuccess = true, Message = message, Data = data };

    public static Result<T> Error(string message)
        => new() { IsSuccess = false, Message = message };
}