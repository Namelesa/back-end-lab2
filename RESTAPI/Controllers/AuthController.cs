using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTAPI.Models.JWTUser;
using RESTAPI.Service;

namespace RESTAPI.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JWTService _jwtService;

    public AuthController(JWTService jwtService)
    {
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel request)
    {
        var result = await _jwtService.Authenticate(request);
        if (result is null)
        {
            return Unauthorized();
        }

        return result;
    }
}