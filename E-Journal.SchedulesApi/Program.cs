using Microsoft.EntityFrameworkCore;
using E_Journal.SchedulesApi.Services;

namespace E_Journal.SchedulesApi;

public static class Program
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        string connectionString = Environment.GetEnvironmentVariable("SCHEDULES_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Environment has not contain the API_DB_CONNECTION_STRING variable that contains database connection string.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptionsAction: options =>
                {
                    options.EnableRetryOnFailure();
                });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<ISchedulesRepository, SchedulesRepository>();
        services.AddHostedService<SchedulesService>();
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