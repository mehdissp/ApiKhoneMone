using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace JWTApi.Api.Response
{

    public class ResponseApi
    {
        public int Status { get; set; }

        public static ResponseApi Ok()
        {
            return new ResponseApi()
            {
                Status = 200
            };
        }

        public static ResponseApi Ok<T>(T data)
        {
            return new DataServiceResponse<T>(data);
        }

        public static ResponseApi Error(string message)
        {
            return new ErrorServiceResponse(message, 400);
        }

        public static ResponseApi Error(string message, int statusCode)
        {
            return new ErrorServiceResponse(message, statusCode);
        }
    }

    public class DataServiceResponse<T> : ResponseApi
    {
        public T Data { get; set; }

        public DataServiceResponse(T data)
        {
            Data = data;
            Status = 200;
        }
    }

    public class ErrorServiceResponse : ResponseApi
    {
        [JsonInclude]
        public string Message { get; set; }

        public ErrorServiceResponse(string message, int code)
        {
            Message = message;
            Status = code;
        }
    }

    public static class ServiceResponseExtensions
    {
        public static IActionResult ToHttpResponse(this ResponseApi response)
        {
            return new ObjectResult(response);
        }
    }
}
