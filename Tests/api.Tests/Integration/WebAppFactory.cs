// =============================================================================
// FILE    : Integration/WebAppFactory.cs
// PURPOSE : Custom WebApplicationFactory for integration tests.
//           Replaces SQL Server with an in-memory EF Core database so that
//           tests are self-contained and require no external infrastructure.
// =============================================================================

using api.models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace api.Tests.Integration
{
    /// <summary>
    /// Factory that boots the real ASP.NET Core pipeline but with an in-memory
    /// database, allowing end-to-end HTTP integration tests without SQL Server.
    /// </summary>
    public class LibraryWebAppFactory : WebApplicationFactory<api.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // ── Remove existing DbContext registrations ──────────────────
                services.RemoveAll<DbContextOptions<ApplicationContext>>();
                services.RemoveAll<ApplicationContext>();
                services.RemoveAll<DbContextOptions<DWContext>>();
                services.RemoveAll<DWContext>();

                // ── Build a clean InMemory-only EF internal service provider ─
                // Program.cs registers both contexts with UseSqlServer. Even
                // after removing DbContextOptions, EF Core still finds SqlServer
                // provider services in the application SP and throws when it
                // also sees InMemory services ("two providers registered").
                //
                // The fix: supply a dedicated InMemory-only IServiceProvider
                // via UseInternalServiceProvider so EF completely ignores the
                // application SP for its provider resolution.
                var efServiceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // ── Register in-memory databases ─────────────────────────────
                var dbName = $"LibraryTestDb_{Guid.NewGuid()}";
                services.AddDbContext<ApplicationContext>(options =>
                    options.UseInMemoryDatabase(dbName)
                           .UseInternalServiceProvider(efServiceProvider));

                services.AddDbContext<DWContext>(options =>
                    options.UseInMemoryDatabase($"DWTestDb_{Guid.NewGuid()}")
                           .UseInternalServiceProvider(efServiceProvider));

                // ── Ensure schema is created ──────────────────────────────────
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                db.Database.EnsureCreated();
            });

            builder.UseEnvironment("Testing");
        }
    }
}
