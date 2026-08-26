using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Verifications;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Verifications
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public VerificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task create(VerificationShahkar verificationShahkar,CancellationToken cancellationToken=default)
        {
            await _context.VerificationShahkars.AddAsync(verificationShahkar, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<VerificationShahkar> GetVerificationShahkar(string nationalCode, string mobileNumber,CancellationToken cancellationToken= default)
        {
            return await _context.VerificationShahkars.Where(s => s.MobileNumber == mobileNumber && s.NationalCode == nationalCode).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
