using ChatApp.API.DTOs;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateRoom([FromBody] RoomDto dto)
    {
        var success = await _roomService.CreateRoomAsync(dto.RoomName);
        if (!success)
            return BadRequest(new { message = "La sala ya existe." });

        return Ok(new { message = "Sala creada exitosamente." });
    }

    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinRoom([FromBody] RoomDto dto)
    {
        var username = HttpContext.User.Identity?.Name ?? "anónimo";

        var success = await _roomService.JoinRoomAsync(username, dto.RoomName);
        if (!success)
            return NotFound(new { message = "La sala no existe." });

        return Ok(new { message = $"Unido a la sala '{dto.RoomName}'." });
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [Authorize]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetRoomByName([FromRoute] string name)
    {
        var room = await _roomService.GetRoomByNameAsync(name);
        if (room == null)
            return NotFound(new { message = "Sala no encontrada" });

        return Ok(new { room });
    }

}
