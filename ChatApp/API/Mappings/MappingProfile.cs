using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Models;

namespace ChatApp.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, RegisterDto>().ReverseMap();
        CreateMap<ChatRoom, RoomDto>().ReverseMap();
    }
}
