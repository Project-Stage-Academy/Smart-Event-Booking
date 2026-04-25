using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartEventBooking.Shared.Configuration;

namespace SmartEventBooking.Infrastructure.Configuration;

internal static class DatabaseConnectionFactory
{
    public static string BuildConnectionString(
        IConfiguration configuration,
        IOptions<DatabaseSettings> settings)
    {
        var fromConfig = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(fromConfig))
        {
            return fromConfig;
        }

        var value = settings.Value;
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
        {
            DataSource = value.Server,
            InitialCatalog = value.Name,
            UserID = value.User,
            Password = value.Password,
            Encrypt = true,
            TrustServerCertificate = value.TrustServerCertificate,
            MultipleActiveResultSets = true
        };

        return builder.ConnectionString;
    }
}
