using Azure;
using JWTApi.Api.Response;
using JWTApi.Application.Services.Payments;
using JWTApi.Application.Services.RealEstateses;
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
    private readonly RealEstatesService _realEstatesService;


    public PaymentController(AppDbContext context, PaymentService paymentGateway, RealEstatesService realEstatesService)
    {
        _context = context;
        _paymentGateway = paymentGateway;
        _realEstatesService = realEstatesService;
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
            
    
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        var gatewayRequest = new PaymentRequest
        {
            Amount =payment.Amount,
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
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
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
                
                //var realEstate=await  _context.RealEstates.FindAsync(payment.RealEstateId);
                //realEstate.Status= RealEstateStatusEnum.Accept;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var wallet = await _paymentGateway.GetBalanceAsync(Guid.Parse(userId));
                await _paymentGateway.DepostiWallet(Guid.Parse(userId), payment.Amount, "شارژ کیف پول", "", verification.RefId);
                payment.Status = PaymentStatus.Success;
                payment.RefId = verification.RefId;
                payment.VerifiedAt = DateTime.UtcNow;
                payment.WalletId = wallet.Data.Id;
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


    [HttpPost("InitializeDepositAds")]
    public async Task<IActionResult> InitializeDepositAds([FromBody] InitPaymentRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

        var result = await _realEstatesService.GetPaymentStatus((int)request.RealEstateId, roleId, userId);
        if (result.Debtor >0)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Amount = (long)result.Debtor,
                Gateway = "Zarinpal",
                Status = PaymentStatus.Pending,
                CallbackUrl = request.CallbackUrl,
                DescriptionRows = request.Description,
                CreatedAt = DateTime.UtcNow,
                Authority = "00",
                RefId = "0",
                UserId = userId,


            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            var gatewayRequest = new PaymentRequest
            {
                Amount = (long)result.Debtor,
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
        return BadRequest(new { error = "با پشتبانی تماس حاصل فرمایید" });
   
    }

    [HttpGet("verify-callback-WithDraw")]
    public async Task<IActionResult> VerifyCallbackWithDraw([FromQuery] string authority, [FromQuery] string status,int adPriceRangeType, [FromQuery] Guid paymentId, [FromQuery] int id,CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
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

                //var realEstate=await  _context.RealEstates.FindAsync(payment.RealEstateId);
                //realEstate.Status= RealEstateStatusEnum.Accept;
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
                var wallet = await _paymentGateway.GetBalanceAsync(Guid.Parse(userId));
              var deposit=  await _paymentGateway.DepostiWallet(Guid.Parse(userId), payment.Amount, "شارژ کیف پول", "", verification.RefId);
                if (deposit.IsSuccess)
                {
                    var check= await _paymentGateway.WithdrawWalletForAd(userId,(AdPriceRangeType)adPriceRangeType, id, roleId, "", cancellationToken);
                    if (check.IsSuccess)
                    {
                  await  _realEstatesService.UpdateStatusRealEstate(id, cancellationToken);
                    }
                }
               
                payment.Status = PaymentStatus.Success;
                payment.RefId = verification.RefId;
                payment.VerifiedAt = DateTime.UtcNow;
                payment.WalletId = wallet.Data.Id;
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

    //[HttpPost("PaymentWithWallet")]
    //public async Task<IActionResult> PaymentWithWallet([FromBody]int id, [FromBody] int adPriceRangeType, CancellationToken cancellationToken)
    //{
    //    var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
    //    var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
    //    if (AdPriceRangeType.InsertAd==(AdPriceRangeType)adPriceRangeType)
    //    {
    //        var gatewayResult = await _paymentGateway.WithdrawWalletForAd(userId, (AdPriceRangeType)adPriceRangeType, id, roleId, "",cancellationToken);
    //        if (gatewayResult.IsSuccess)
    //        {
    //            await _realEstatesService.UpdateStatusRealEstate(id, cancellationToken);
    //        }
    //    }
    //    else if ((AdPriceRangeType)adPriceRangeType == AdPriceRangeType.ShowApplicantRequest)
    //    {
    //        var gatewayResult = await _paymentGateway.WithdrawWalletForAd(userId, (AdPriceRangeType)adPriceRangeType, id, roleId, "", cancellationToken);
    //        if (gatewayResult.IsSuccess)
    //        {
    //            await _realEstatesService.UpdateStatusRealEstate(id, cancellationToken);
    //        }
    //    }

    //        return ResponseApi.Ok().ToHttpResponse();
    //}
    [HttpPost("PaymentWithWallet")]
    public async Task<IActionResult> PaymentWithWallet([FromBody] PaymentRequestDTOS request, CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

        if (AdPriceRangeType.InsertAd == (AdPriceRangeType)request.AdPriceRangeType)
        {
            var gatewayResult = await _paymentGateway.WithdrawWalletForAd(userId, (AdPriceRangeType)request.AdPriceRangeType, request.Id, roleId, "", cancellationToken);
            if (gatewayResult.IsSuccess)
            {
                await _realEstatesService.UpdateStatusRealEstate(request.Id, cancellationToken);
            }
        }
        else if ((AdPriceRangeType)request.AdPriceRangeType == AdPriceRangeType.ShowApplicantRequest)
        {
            var gatewayResult = await _paymentGateway.WithdrawWalletForAd(userId, (AdPriceRangeType)request.AdPriceRangeType, request.Id, roleId, "", cancellationToken);
            if (gatewayResult.IsSuccess)
            {
                await _paymentGateway.AccessToShowMobileNumber(request.Id, roleId, userId, cancellationToken);
            }
        }

        return ResponseApi.Ok().ToHttpResponse();
    }
    public class PaymentRequestDTOS
    {
        public int Id { get; set; }
        public int AdPriceRangeType { get; set; }
    }


}