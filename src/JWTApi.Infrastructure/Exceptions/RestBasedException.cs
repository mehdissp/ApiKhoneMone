using JWTApi.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Exceptions
{
    public class RestBasedException : TrackableException
    {
        public int HttpStatus { get; set; }

        public RestBasedException(string message, int httpStatus = 403, string transactionId = "")
            : base(message, transactionId)
        {
            HttpStatus = httpStatus;
        }

        public RestBasedException(ErrorCodeDto error, string transactionId = "")
            : base(error.Message, transactionId)
        {
            HttpStatus = error.Code;
        }
    }
}
