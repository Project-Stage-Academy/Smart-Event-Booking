namespace SmartEventBooking.Shared.Configuration;

public sealed class DatabaseSettings
{
    public const string SectionName = "Database";

    public string Server { get; set; } = "localhost,1433";
    public string Name { get; set; } = "SmartEventBookingDb";
    public string User { get; set; } = "sa";
    public string Password { get; set; } = string.Empty;
    public bool TrustServerCertificate { get; set; } = true;
}
