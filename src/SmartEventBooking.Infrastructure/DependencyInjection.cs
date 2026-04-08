using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartEventBooking.Infrastructure.Configuration;
using SmartEventBooking.Infrastructure.Persistence;
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
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
            .PostConfigure(options =>
            {
                options.Server = GetEnvOrDefault("DB_SERVER", options.Server);
                options.Name = GetEnvOrDefault("DB_NAME", options.Name);
                options.User = GetEnvOrDefault("DB_USER", options.User);
                options.Password = GetEnvOrDefault("DB_PASSWORD", options.Password);

                var trustServerCertificateValue = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE");
                if (string.IsNullOrWhiteSpace(trustServerCertificateValue))
                {
                    return;
                }

                if (!bool.TryParse(trustServerCertificateValue, out var trustServerCertificate))
                {
                    throw new OptionsValidationException(
                        nameof(DatabaseSettings),
                        typeof(DatabaseSettings),
                        ["DB_TRUST_SERVER_CERTIFICATE must be 'true' or 'false'."]);
                }

                options.TrustServerCertificate = trustServerCertificate;
            })
            .Validate(options => !string.IsNullOrWhiteSpace(options.Server), "Database:Server is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Name), "Database:Name is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.User), "Database:User is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "Database:Password is required.")
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>();
            var connectionString = DatabaseConnectionFactory.BuildConnectionString(configuration, settings);
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    private static string GetEnvOrDefault(string environmentVariable, string currentValue)
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(environmentVariable);
        return string.IsNullOrWhiteSpace(fromEnvironment)
            ? currentValue
            : fromEnvironment;
    }
}
