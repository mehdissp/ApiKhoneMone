using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Application.DTOs;
using JWTApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("registerNewUser")]
        public async Task<IActionResult> RegisterNewUser(RegisterNewUserDto dto, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var (success, message) = await _userService.RegisterAsync(dto, userId, cancellationToken);
            //return success ? Ok(message) : BadRequest(message);
                    return success
           ? ResponseApi.Ok(message).ToHttpResponse()
           : BadRequest(message);
        }
        [HttpPost("updateNewUser")]
        public async Task<IActionResult> updateNewUser(UpdateNewUserDto dto, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var (success, message) = await _userService.UpdateUserAsync(dto, userId, cancellationToken);
            //return success ? Ok(message) : BadRequest(message);
            return success
   ? ResponseApi.Ok(message).ToHttpResponse()
   : BadRequest(message);
        }

        [HttpPost("GetUsers")]
        public async Task<IActionResult> GetUsers([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _userService.GetNewUser(userId, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
      
            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }
        [HttpPost("GetUsersCombo")]
        public async Task<IActionResult> GetUsersCombo([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _userService.GetUsersForComboAsync(userId, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
      
            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }


        [HttpGet("GetRoleCombo")]
        public async Task<IActionResult> GetRoleCombo(CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _userService.GetRole( cancellationToken);
          
            return ResponseApi.Ok(result).ToHttpResponse();

        }


        [HttpPost("GetProjectUsers")]
        public async Task<IActionResult> GetProjectUsers([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _userService.GetProjectUserDtosAsync(userId, (int)pageSize.Id, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
             
            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }

    }
}
