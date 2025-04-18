using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using ChatApp.API.Services;
using ChatApp.API.Models;
using ChatApp.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly ChatAppDbContext _context;

    public AuthServiceTests()
    {
        // Configuración en memoria
        var options = new DbContextOptionsBuilder<ChatAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChatAppDbContext(options);

        // Configuración JWT simulada
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("THIS_IS_A_SUPER_SECRET_KEY_FOR_TESTS_2024!");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("ChatApp");

        // Mapper vacío (no usamos en AuthService directamente)
        var mockMapper = new Mock<IMapper>();

        _authService = new AuthService(_context, mockConfig.Object, mockMapper.Object);
    }

    [Fact]
    public async Task Register_NewUser_ShouldSucceed()
    {
        var result = await _authService.RegisterAsync(new RegisterDto
        {
            Username = "jhon",
            Password = "1234"
        });

        Assert.True(result);
    }

    [Fact]
    public async Task Register_ExistingUser_ShouldFail()
    {
        await _authService.RegisterAsync(new RegisterDto { Username = "jhon", Password = "1234" });

        var result = await _authService.RegisterAsync(new RegisterDto
        {
            Username = "jhon",
            Password = "otro"
        });

        Assert.False(result);
    }

    [Fact]
    public async Task Login_CorrectCredentials_ShouldReturnToken()
    {
        await _authService.RegisterAsync(new RegisterDto { Username = "jhon", Password = "1234" });

        var token = await _authService.LoginAsync(new LoginDto
        {
            Username = "jhon",
            Password = "1234"
        });

        Assert.NotNull(token);
    }

    [Fact]
    public async Task Login_WrongPassword_ShouldReturnNull()
    {
        await _authService.RegisterAsync(new RegisterDto { Username = "jhon", Password = "1234" });

        var token = await _authService.LoginAsync(new LoginDto
        {
            Username = "jhon",
            Password = "wrongpass"
        });

        Assert.Null(token);
    }

    [Fact]
    public void ValidateToken_ValidToken_ShouldReturnTrue()
    {
        var user = new User { Id = 1, Username = "jhon", PasswordHash = "any" };
        var token = _authService.GetType()
            .GetMethod("GenerateJwt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_authService, new object[] { user })!.ToString();

        var isValid = _authService.ValidateToken(token!);

        Assert.True(isValid);
    }
}
