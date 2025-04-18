using ChatApp.API.DTOs;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto);
        if (!success)
            return BadRequest(new { message = "El usuario ya existe." });

        return Ok(new { message = "Usuario registrado correctamente." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        if (token == null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        return Ok(new { token });
    }

    // === ENDPOINTS USADOS POR MOSQUITTO-GO-AUTH (modo jwt/remote) ===

    [HttpPost]
    public IActionResult Auth()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return BadRequest("Authorization header missing or malformed");

        var token = authHeader.Replace("Bearer ", "");

        var isValid = _authService.ValidateToken(token);
        return isValid ? Ok() : Unauthorized();
    }


    [HttpPost("superuser")]
    public IActionResult Superuser([FromForm] string username)
    {
        return Ok("deny"); // Ningún usuario es superuser (por ahora)
    }

    [HttpPost("acl")]
    public IActionResult Acl([FromForm] string username, [FromForm] string topic, [FromForm] string acc)
    {
        return Ok("ok"); // Permite acceso a todos los topics por ahora
    }
}
