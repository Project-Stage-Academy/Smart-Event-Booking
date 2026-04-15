using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartEventBooking.Application.Abstractions.Data;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Infrastructure.Configuration;
using SmartEventBooking.Infrastructure.Identity;
using SmartEventBooking.Infrastructure.Persistence;
using SmartEventBooking.Infrastructure.Persistence.Repositories;
using SmartEventBooking.Shared.Configuration;

namespace SmartEventBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.SectionName));

        services
            .AddOptions<DatabaseSettings>()
            .BindConfiguration(DatabaseSettings.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Server), "Database:Server is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Name), "Database:Name is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.User), "Database:User is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "Database:Password is required.")
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>();
            var connectionString = DatabaseConnectionFactory.BuildConnectionString(configuration, settings);
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            });
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<IAuthService, AuthService>();
        
        services.AddScoped<IDatabaseSeeder, IdentitySeeder>();


        return services;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
}
