using Application.Features.Identity.Tokens;
using Application.Features.Identity.Tokens.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Infrastructure.OpenAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseApiController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to obtain jwt for login.")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var response = await Sender.Send(new GetTokenQuery { TokenRequest = tokenRequest });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("refresh-token")]
        [OpenApiOperation("Used to obtain refresh jwt for login")]
        [ShouldHavePermission(action:SchoolConstants.RefreshToken,feature:SchoolFeature.Tokens)]
        public async Task<IActionResult> GetfRefreshTokeAsync([FromBody] RefreshTokenRequest request)
        {
            var response = await Sender.Send(new GetRefreshTokenQuery { RefreshToken = request });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
