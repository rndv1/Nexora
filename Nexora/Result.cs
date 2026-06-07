namespace Nexora
{
    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
        public static new Result<T> Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; } = string.Empty;
        public static Result Success() => new() { IsSuccess = true };
        public static Result Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };

        public static implicit operator bool(Result result) => result.IsSuccess;
    }
}
