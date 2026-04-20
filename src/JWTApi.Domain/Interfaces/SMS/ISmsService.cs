using JWTApi.Domain.Dtos.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.SMS
{
    public interface ISmsService
    {
        Task<SmsSendResult> SendVerificationCodeAsync(string mobile, string code, int templateId = 100000);
    }
}
