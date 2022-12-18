using Microsoft.EntityFrameworkCore;
using E_Journal.JournalApi.Services;
using E_Journal.JournalApi.Models;

namespace E_Journal.JournalApi;

public static class Program
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        string connectionString = Environment.GetEnvironmentVariable("JOURNAL_DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Environment has not contain the JOURNAL_DB_CONNECTION_STRING variable that contains database connection string.");

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

        services.AddScoped<IBaseRepository<Group>, BaseRepository<Group>>();
        services.AddScoped<IBaseRepository<Teacher>, BaseRepository<Teacher >>();
        services.AddScoped<IBaseRepository<Models.Type>, BaseRepository<Models.Type>>();
        services.AddScoped<IBaseRepository<Teacher>, BaseRepository<Teacher>>();
        services.AddScoped<IBaseRepository<ScoreValue>, BaseRepository<ScoreValue>>();
        services.AddScoped<IScoresRepository, ScoresRepository>();
    }

    public static void Configure(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureServices();

        var app = builder.Build();

        app.Configure();

        app.Run();
    }
}
