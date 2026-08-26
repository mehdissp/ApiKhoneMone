using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Verifications
{
    public interface IVerificationRepository
    {
        Task create(VerificationShahkar verificationShahkar, CancellationToken cancellationToken = default);
        Task<VerificationShahkar> GetVerificationShahkar(string nationalCode,string mobileNumber, CancellationToken cancellationToken = default);
    }
}
