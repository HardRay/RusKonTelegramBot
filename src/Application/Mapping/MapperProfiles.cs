﻿using Application.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Application.Mapping;

public sealed class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<User, UserModel>()
            .ReverseMap();
        CreateMap<VacancyFilter, VacancyFilterModel>()
            .ReverseMap();

        CreateMap<Message, MessageModel>()
            .ReverseMap();

        CreateMap<Vacancy, VacancyModel>()
            .ReverseMap();
        CreateMap<AdditionalAtribute, AdditionalAtributeModel>()
            .ReverseMap();

        CreateMap<Subscription, SubscriptionModel>()
            .ReverseMap();

        CreateMap<City, CityModel>()
            .ReverseMap();

        CreateMap<Direction, DirectionModel>()
            .ReverseMap();
    }
}
