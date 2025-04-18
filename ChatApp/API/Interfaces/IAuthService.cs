using ChatApp.API.DTOs;

namespace ChatApp.API.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
    bool ValidateToken(string token);
}
