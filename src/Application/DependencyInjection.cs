using Application.Interfaces.Services;
using Application.Mapping;
using Application.Serivices;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MapperProfiles));

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IMessageService, MessageService>();

        return services;
    }
}
