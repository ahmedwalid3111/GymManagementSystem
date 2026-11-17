using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Helpers
{
    public sealed record Result(bool Success,string? Error=null, ResultKind Kind=ResultKind.Ok)
    {
        public static Result Ok()
        {
            return new Result(true);
        }
        public  static Result Fail(string error, ResultKind kind = ResultKind.Conflict)
        {
            return new Result(false, error, kind);
        }
        public static Result NotFound(string error="Not Found")
        {
            return new Result(false, error, ResultKind.NotFound);
        }
        public static Result Validation(string error)
        {
            return new Result(false, error, ResultKind.ValidationFailed);
        }
    }
    public sealed record Result<T>(bool Success, T? Value , string? Error = null, ResultKind Kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T value)
        {
            return new Result<T>(true, value);
        }
        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict)
        {
            return new Result<T>(false, default, error, kind);
        }
        public static Result<T> NotFound(string error = "Not Found")
        {
            return new Result<T>(false, default, error, ResultKind.NotFound);
        }
        
    }
    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden,
    }
}
