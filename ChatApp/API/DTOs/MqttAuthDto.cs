namespace ChatApp.API.DTOs;

public class MqttAuthDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!; // Aquí llega el JWT
}
