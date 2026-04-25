using Crm.Application.DTOs.Auth;
using Crm.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _service.Login(request);
        if (result.IsFailed)
            return Unauthorized();
        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        var result = await _service.Register(request);
        if (result.IsFailed)
            return BadRequest();
        return Ok(result.Value);
    }
}