using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Data;
using Application.Interfaces.Common;

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
