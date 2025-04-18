using ChatApp.API.DTOs;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("[controller]")]
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

    // Endpoint usado por Mosquitto para validar JWT
    [HttpPost("auth")]
    public IActionResult MqttAuth([FromBody] MqttAuthDto dto)
    {
        var isValid = _authService.ValidateToken(dto.Password);
        if (!isValid)
            return Unauthorized(new { result = "deny" });

        return Ok(new { result = "ok" });
    }
}
