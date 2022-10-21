using Microsoft.EntityFrameworkCore;
using E_Journal.SchedulesApi.Services;

namespace E_Journal.SchedulesApi;

public static class Program
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        string connectionString = Environment.GetEnvironmentVariable("SCHEDULES_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Environment has not contain the API_DB_CONNECTION_STRING variable that contains database connection string.");

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptionsAction: options =>
                {
                    options.EnableRetryOnFailure();
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<ISchedulesRepository, SchedulesRepository>();
    }

    public static void Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureServices();

        var app = builder.Build();

        app.Configure();
        
        app.MapControllers();
        
        app.Run();
    }
}