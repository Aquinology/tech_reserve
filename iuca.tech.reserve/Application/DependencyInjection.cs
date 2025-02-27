using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AppMappingProfile));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IImportDataService, ImportDataService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IEquipmentRequestService, EquipmentRequestService>();

        return services;
    }
}