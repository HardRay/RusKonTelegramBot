using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public sealed class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<User, UserModel>()
            .ReverseMap();
    }
}
