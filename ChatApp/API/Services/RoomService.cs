using ChatApp.API.Interfaces;
using ChatApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Services;

public class RoomService : IRoomService
{
    private readonly ChatAppDbContext _context;

    public RoomService(ChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateRoomAsync(string roomName)
    {
        var exists = await _context.ChatRooms.AnyAsync(r => r.Name == roomName);
        if (exists) return false;

        var room = new ChatRoom { Name = roomName };
        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> JoinRoomAsync(string username, string roomName)
    {
        // Por ahora solo validamos que la sala exista
        var room = await _context.ChatRooms.FirstOrDefaultAsync(r => r.Name == roomName);
        if (room == null) return false;

        // En el futuro podrías guardar la relación usuario/sala
        return true;
    }

    public async Task<List<string>> GetAllRoomsAsync()
    {
        return await _context.ChatRooms
            .Select(r => r.Name)
            .ToListAsync();
    }
}
