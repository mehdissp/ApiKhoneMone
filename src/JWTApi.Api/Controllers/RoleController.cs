using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Application.Services;

using JWTApi.Application.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {

        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("GetRoles")]
        public async Task<IActionResult> GetRoles([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _roleService.GetRolesAsync( cancellationToken);
            var response = new
            {
                Items = result,
                TotalCount = 10,
                TotalPages = 1,
               
            };
            return ResponseApi.Ok(response).ToHttpResponse();

        }

    }
}
