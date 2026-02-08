namespace JWTApi.Api.Response
{
    public class BaseResult : Result<int?>
    {
        public static BaseResult Success()
        {
            return new BaseResult { succeed = true };
        }
        public static new BaseResult Failure(string errorMessage = "")
        {
            return new BaseResult { ErrorMessage = errorMessage };
        }
    }
}
