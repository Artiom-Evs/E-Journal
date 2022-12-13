using Microsoft.EntityFrameworkCore;
using E_Journal.SchedulesApi.Services;
using E_Journal.SchedulesApi.Models;
using Microsoft.AspNetCore.Hosting.Server;

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

        services.AddScoped<IBaseRepository<Group>, BaseRepository<Group>>();
        services.AddScoped<IBaseRepository<Subject>, BaseRepository<Subject>>();
        services.AddScoped<IBaseRepository<Models.Type>, BaseRepository<Models.Type>>();
        services.AddScoped<IBaseRepository<Teacher>, BaseRepository<Teacher>>();
        services.AddScoped<IBaseRepository<Room>, BaseRepository<Room>>();
        services.AddScoped<ILessonsRepository, LessonsRepository>();
        services.AddScoped<IParserService, ParserService>();

        services.AddScoped<IWebAccessorService, WebAccessorService>();
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddHostedService<UpdateHostedService>();
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

        app.MapControllers();
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