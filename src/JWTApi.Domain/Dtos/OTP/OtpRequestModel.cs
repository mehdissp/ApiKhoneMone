using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.OTP
{
    public class OtpRequestModel
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int AttemptCount { get; set; }
    }

    public class OtpVerificationModel
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
    }

    public class SmsSendResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public long? SmsId { get; set; }
    }
}
