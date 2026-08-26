using JWTApi.Api.Response;
using JWTApi.Application.Services.Payments;
using JWTApi.Application.Services.RealEstateses;
using JWTApi.Application.Services.Stories;
using JWTApi.Application.Services.VerificationService;
using JWTApi.Domain.Entities;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {

        private readonly VerificationService _verificationService;


        public VerificationController(VerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpPost("VerficationShahkarLite")]
        [PublicEndpoint]
        public async Task<IActionResult> VerficationShahkarLite([FromQuery]string mobileNumber,string nationalCode, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _verificationService.ShahkarLite(mobileNumber, nationalCode, userId);


            return ResponseApi.Ok(result).ToHttpResponse();
        }

    }
}
