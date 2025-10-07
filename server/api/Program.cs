using System.ComponentModel.DataAnnotations;
using api.Services;
using efscaffold;
using Microsoft.EntityFrameworkCore;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });
        services.AddDbContext<MyDbContext>((services, options) =>
        {
            options.UseNpgsql(services.GetRequiredService<AppOptions>().DbConnectionString);
        });
        
        services.AddControllers()
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        
        services.AddOpenApiDocument();
        services.AddCors();
        
        
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<ISeeder, Seeder>();
        services.AddExceptionHandler<MyGlobalExceptionHandler>();
    }

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services);
        var app = builder.Build();


        var appOptions = app.Services.GetRequiredService<AppOptions>();
        //Here im just checking that I can get the "Db" connection string - it throws exception if not minimum 1 length
        Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);
        app.UseExceptionHandler(config => { });
        app.UseOpenApi();
        app.UseSwaggerUi();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));
        app.MapControllers();
        app.GenerateApiClientsFromOpenApi("/../../client/src/generated-client.ts").GetAwaiter().GetResult();
        if (app.Environment.IsDevelopment())
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<ISeeder>();
                if (seeder != null) seeder.Seed();
            }

        app.Run();
    }
}