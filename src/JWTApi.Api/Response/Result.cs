namespace JWTApi.Api.Response
{
    public class Result<T>
    {
        public bool succeed { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; }
        public static Result<T> Success(T? data)
        {
            return new Result<T> { succeed = true, Data = data };
        }
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T> { ErrorMessage = errorMessage };
        }
    }
}
