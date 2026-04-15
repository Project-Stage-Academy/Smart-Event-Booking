using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartEventBooking.Application.Interfaces.Repositories;
using SmartEventBooking.Infrastructure.Configuration;
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

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
