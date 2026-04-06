using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.Configure<DatabaseSettings>(options =>
        {
            options.Server =
                Environment.GetEnvironmentVariable("DB_SERVER")
                ?? configuration[$"{DatabaseSettings.SectionName}:Server"]
                ?? "localhost,1433";

            options.Name =
                Environment.GetEnvironmentVariable("DB_NAME")
                ?? configuration[$"{DatabaseSettings.SectionName}:Name"]
                ?? "SmartEventBookingDb";

            options.User =
                Environment.GetEnvironmentVariable("DB_USER")
                ?? configuration[$"{DatabaseSettings.SectionName}:User"]
                ?? "sa";

            options.Password =
                Environment.GetEnvironmentVariable("DB_PASSWORD")
                ?? configuration[$"{DatabaseSettings.SectionName}:Password"]
                ?? string.Empty;

            var trustServerCertificateValue =
                Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE")
                ?? configuration[$"{DatabaseSettings.SectionName}:TrustServerCertificate"];

            if (bool.TryParse(trustServerCertificateValue, out var trustServerCertificate))
            {
                options.TrustServerCertificate = trustServerCertificate;
            }
        });

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>();
            var connectionString = DatabaseConnectionFactory.BuildConnectionString(configuration, settings);
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
