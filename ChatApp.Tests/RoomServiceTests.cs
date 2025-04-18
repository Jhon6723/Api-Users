using Xunit;
using ChatApp.API.Models;
using ChatApp.API.Services;
using ChatApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatApp.Tests;

public class RoomServiceTests
{
    private readonly ChatAppDbContext _context;
    private readonly IRoomService _roomService;

    public RoomServiceTests()
    {
        var options = new DbContextOptionsBuilder<ChatAppDbContext>()
            .UseInMemoryDatabase(databaseName: "RoomServiceTestDB_" + Guid.NewGuid())
            .Options;

        _context = new ChatAppDbContext(options);
        _roomService = new RoomService(_context);
    }

    [Fact]
    public async Task CreateRoom_NewRoom_ShouldSucceed()
    {
        var result = await _roomService.CreateRoomAsync("devs");
        Assert.True(result);
    }

    [Fact]
    public async Task CreateRoom_ExistingRoom_ShouldFail()
    {
        await _roomService.CreateRoomAsync("devs");
        var result = await _roomService.CreateRoomAsync("devs");
        Assert.False(result);
    }

    [Fact]
    public async Task JoinRoom_ExistingRoom_ShouldSucceed()
    {
        await _roomService.CreateRoomAsync("gamers");
        var result = await _roomService.JoinRoomAsync("jhon", "gamers");
        Assert.True(result);
    }

    [Fact]
    public async Task JoinRoom_NonExistingRoom_ShouldFail()
    {
        var result = await _roomService.JoinRoomAsync("jhon", "noExiste");
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllRooms_ShouldReturnList()
    {
        await _roomService.CreateRoomAsync("chat1");
        await _roomService.CreateRoomAsync("chat2");

        var rooms = await _roomService.GetAllRoomsAsync();

        Assert.Contains("chat1", rooms);
        Assert.Contains("chat2", rooms);
        Assert.Equal(2, rooms.Count);
    }
}
