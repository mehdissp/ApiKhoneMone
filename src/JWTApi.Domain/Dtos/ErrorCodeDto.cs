using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos
{
    public class ErrorCodeDto
    {
        //public static ErrorCodeDto Create(string code, string message) => new ErrorCodeDto { Code = code, Message = message };

        public static ErrorCodeDto Create(int code, string message) => new ErrorCodeDto { Code = code, Message = message };

        public int Code { get; private set; }
        public string Message { get; private set; }

        // public static ErrorCodeDto None 
        //     = new ErrorCodeDto { Code = string.Empty, Message = String.Empty };

        public string GetMessage(params string[] args) => string.Format(Message, args);
    }
}
