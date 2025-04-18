using ChatApp.API.DTOs;

namespace ChatApp.API.Interfaces;

public interface IRoomService
{
    Task<bool> CreateRoomAsync(string roomName);
    Task<bool> JoinRoomAsync(string username, string roomName);
    Task<List<string>> GetAllRoomsAsync();
}
