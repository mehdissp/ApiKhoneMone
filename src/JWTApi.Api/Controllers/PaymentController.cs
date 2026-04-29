using JWTApi.Api.Response;
using JWTApi.Application.Services.Payments;
using JWTApi.Domain.Dtos.Payments;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Payments;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PaymentService _paymentGateway;

    public PaymentController(AppDbContext context, PaymentService paymentGateway)
    {
        _context = context;
        _paymentGateway = paymentGateway;
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize([FromBody] InitPaymentRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Gateway = "Zarinpal",
            Status = PaymentStatus.Pending,
            CallbackUrl = request.CallbackUrl,
            DescriptionRows = request.Description,
            CreatedAt = DateTime.UtcNow,
            Authority="00",
            RefId="0",
            UserId= userId,
            RealEstateId=request.RealEstateId
    
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        var gatewayRequest = new PaymentRequest
        {
            Amount = request.Amount,
            CallbackUrl = $"{request.CallbackUrl}?paymentId={payment.Id}",
            Description = request.Description
        };

        var gatewayResult = await _paymentGateway.RequestPaymentAsync(gatewayRequest);

        if (!gatewayResult.IsSuccess)
        {
            return BadRequest(new { error = gatewayResult.ErrorMessage });
        }

        payment.Authority = gatewayResult.Authority;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            paymentId = payment.Id,
            gatewayUrl = gatewayResult.GatewayUrl
        });
    }

    [HttpGet("verify-callback")]
    public async Task<IActionResult> VerifyCallback([FromQuery] string authority, [FromQuery] string status, [FromQuery] Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
        {
            return ResponseApi.Error("پرداختی یافت نشد").ToHttpResponse();
        }
        if (payment.Status == PaymentStatus.Success)
        {

            //return Redirect($"{payment.CallbackUrl}?status=success&refId={payment.RefId}");
            return ResponseApi.Ok(new { refId = payment.RefId, isSuccess = true }).ToHttpResponse();
        }

        if (status == "OK")
        {
            var verification = await _paymentGateway.VerifyPaymentAsync(new VerificationRequest
            {
                Amount = payment.Amount,
                Authority = authority
            });

            if (verification.IsSuccess)
            {
                var realEstate=await  _context.RealEstates.FindAsync(payment.RealEstateId);
                realEstate.Status= RealEstateStatusEnum.Accept;
                payment.Status = PaymentStatus.Success;
                payment.RefId = verification.RefId;
                payment.VerifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                //                return Redirect($"{payment.CallbackUrl}?status=success&refId={verification.RefId}");
                return ResponseApi.Ok(new { refId = verification.RefId, isSuccess = true }).ToHttpResponse();
            }
        }

        payment.Status = PaymentStatus.Failed;
        await _context.SaveChangesAsync();
        //return Redirect($"{payment.CallbackUrl}?status=failed");
        return ResponseApi.Error("SS").ToHttpResponse();
    }

    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetPaymentStatus(Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null)
            return NotFound();

        return Ok(new
        {
            payment.Status,
            payment.RefId,
            payment.Amount,
            StatusText = payment.Status switch
            {
                PaymentStatus.Pending => "در انتظار پرداخت",
                PaymentStatus.Success => "پرداخت موفق",
                PaymentStatus.Failed => "پرداخت ناموفق",
                _ => "نامشخص"
            }
        });
    }
}