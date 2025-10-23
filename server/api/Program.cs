using System.ComponentModel.DataAnnotations;
using api.Services;
using efscaffold;
using Microsoft.EntityFrameworkCore;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // App options
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });

        // DbContext
        services.AddDbContext<MyDbContext>((services, options) =>
        {
            options.UseNpgsql(services.GetRequiredService<AppOptions>().DbConnectionString);
        });

        // Controllers
        services.AddControllers()
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

        // OpenAPI / Swagger
        services.AddOpenApiDocument();
        services.AddCors();

        // Services
        services.AddScoped<ILibraryService, LibraryService>();

        // Exception handler
        services.AddExceptionHandler<MyGlobalExceptionHandler>();
    }

    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services);
        var app = builder.Build();

        // Validate app options
        var appOptions = app.Services.GetRequiredService<AppOptions>();
        Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);

        app.UseExceptionHandler(config => { });
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));
        app.UseOpenApi();
        app.UseSwaggerUi();
        app.MapControllers();

        // Generate client
        await app.GenerateApiClientsFromOpenApi("/../../client/src/generated-client.ts");

        // Seed database in development
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            var seeder = new Seeder(dbContext);
            await seeder.Seed();
        }

        // Run app
        await app.RunAsync();
    }
}
