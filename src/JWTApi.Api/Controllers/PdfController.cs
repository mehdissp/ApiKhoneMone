using JWTApi.Application.Services.RealEstateses;
using JWTApi.Infrastructure.Middleware;
using JWTApi.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PdfController : ControllerBase
    {
        private readonly RealEstatesService _realEstatesService;
        private readonly IPdfGeneratorService _pdfGeneratorService;

        public PdfController(RealEstatesService realEstatesService, IPdfGeneratorService pdfGeneratorService)
        {
            _realEstatesService = realEstatesService;
            _pdfGeneratorService = pdfGeneratorService;
        }

        [HttpGet("GeneratePropertyPdf")]
        [PublicEndpoint]
        public async Task<IActionResult> GeneratePropertyPdf(int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _realEstatesService.GetRealEstateDetails(id, userId, cancellationToken);

            if (result == null)
                return NotFound("ملک یافت نشد");

            var pdfBytes = await _pdfGeneratorService.GeneratePropertyPdf(result, cancellationToken);

            return File(pdfBytes, "application/pdf", $"Property_{result.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
    }
}
