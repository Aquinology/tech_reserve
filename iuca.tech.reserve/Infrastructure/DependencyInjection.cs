using Application.Interfaces.Common;
using Ardalis.GuardClauses;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurations
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        var googleConfiguration = configuration.GetSection("Authentication:Google");

        Guard.Against.Null(googleConfiguration, message: "Configuration 'Authentication:Google' not found.");

        // Database
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        // File service
        services.AddScoped<IFileService, FileService>();

        // Seeds
        services.AddScoped<ApplicationDbContextInitialiser>();

        // Identity
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Google authentication
        services
            .AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = googleConfiguration["ClientId"]!;
                options.ClientSecret = googleConfiguration["ClientSecret"]!;
                options.CallbackPath = "/signin-google";
            });

        return services;
    }
}
