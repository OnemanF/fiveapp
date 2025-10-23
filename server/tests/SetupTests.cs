using api;
using efscaffold;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Reuse the main API’s service setup
        Program.ConfigureServices(services);

        // Replace the normal DbContext with a PostgreSQL test container
        services.RemoveAll(typeof(MyDbContext));

        services.AddScoped<MyDbContext>(factory =>
        {
            var postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15-alpine")
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            postgreSqlContainer.StartAsync().GetAwaiter().GetResult();

            var connectionString = postgreSqlContainer.GetConnectionString();
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var ctx = new MyDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        });

        // ✅ Register Seeder so it can be injected
        services.AddScoped<Seeder>();
    }
}