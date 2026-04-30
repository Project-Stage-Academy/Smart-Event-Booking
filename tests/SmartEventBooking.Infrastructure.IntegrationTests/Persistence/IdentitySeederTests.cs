using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartEventBooking.Application.Abstractions.Data;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Infrastructure.Identity;
using SmartEventBooking.Infrastructure.Persistence;
using SmartEventBooking.Infrastructure.Persistence.Repositories;
using SmartEventBooking.Shared.Configuration;
using SmartEventBooking.Shared.Constants;

namespace SmartEventBooking.Infrastructure.IntegrationTests.Persistence;

public class IdentitySeederTests
{
    [Fact]
    public async Task SeedAsync_ShouldCreateRolesAdminIdentityUserAndDomainUser_WhenAdminSettingsAreConfigured()
    {
        await using var connection = CreateOpenInMemoryConnection();
        await using var provider = BuildServiceProvider(connection, "admin@test.local", "AdminPassword123!");

        await EnsureDatabaseCreatedAsync(provider);
        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = await dbContext.Roles.Select(x => x.Name).ToListAsync();
        roles.Should().BeEquivalentTo(new[] { RoleConstants.Admin, RoleConstants.User });

        var appUser = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == "admin@test.local");
        appUser.Should().NotBeNull();
        appUser!.EmailConfirmed.Should().BeTrue();

        var roleNames = await userManager.GetRolesAsync(appUser);
        roleNames.Should().ContainSingle().Which.Should().Be(RoleConstants.Admin);

        var domainUser = await dbContext.DomainUsers.SingleOrDefaultAsync(x => x.Id == appUser.Id);
        domainUser.Should().NotBeNull();
        domainUser!.FirstName.Should().Be("System");
        domainUser.LastName.Should().Be("Administrator");
    }

    [Fact]
    public async Task SeedAsync_ShouldBeIdempotent_WhenInvokedMultipleTimes()
    {
        await using var connection = CreateOpenInMemoryConnection();
        await using var provider = BuildServiceProvider(connection, "admin@test.local", "AdminPassword123!");

        await EnsureDatabaseCreatedAsync(provider);
        await SeedAsync(provider);
        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var seededRolesCount = await dbContext.Roles.CountAsync(
            x => x.Name == RoleConstants.Admin || x.Name == RoleConstants.User);
        seededRolesCount.Should().Be(2);

        var appUsersCount = await dbContext.Users.CountAsync(x => x.Email == "admin@test.local");
        appUsersCount.Should().Be(1);

        var appUser = await dbContext.Users.SingleAsync(x => x.Email == "admin@test.local");
        var domainUsersCount = await dbContext.DomainUsers.CountAsync(x => x.Id == appUser.Id);
        domainUsersCount.Should().Be(1);
    }

    [Fact]
    public async Task SeedAsync_ShouldCreateOnlyRoles_WhenAdminSettingsAreMissing()
    {
        await using var connection = CreateOpenInMemoryConnection();
        await using var provider = BuildServiceProvider(connection, string.Empty, string.Empty);

        await EnsureDatabaseCreatedAsync(provider);
        await SeedAsync(provider);

        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var roles = await dbContext.Roles.Select(x => x.Name).ToListAsync();
        roles.Should().BeEquivalentTo(new[] { RoleConstants.Admin, RoleConstants.User });

        (await dbContext.Users.CountAsync()).Should().Be(0);
        (await dbContext.DomainUsers.CountAsync()).Should().Be(0);
    }

    private static SqliteConnection CreateOpenInMemoryConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }

    private static ServiceProvider BuildServiceProvider(
        SqliteConnection connection,
        string adminEmail,
        string adminPassword)
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.Configure<AdminOptions>(options =>
        {
            options.Email = adminEmail;
            options.Password = adminPassword;
        });

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDatabaseSeeder, IdentitySeeder>();

        return services.BuildServiceProvider(validateScopes: true);
    }

    private static async Task EnsureDatabaseCreatedAsync(ServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    private static async Task SeedAsync(ServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
}
