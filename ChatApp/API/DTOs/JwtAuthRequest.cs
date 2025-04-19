namespace ChatApp.API.DTOs;

public class JwtAuthRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
