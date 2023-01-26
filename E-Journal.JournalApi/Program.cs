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

        services.AddHealthChecks();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IBaseRepository<Group>, BaseRepository<Group>>();
        services.AddScoped<IBaseRepository<Subject>, BaseRepository<Subject>>();
        services.AddScoped<IBaseRepository<Teacher>, BaseRepository<Teacher>>();
        services.AddScoped<IBaseRepository<TrainingType>, BaseRepository<TrainingType>>();
        services.AddScoped<IBaseRepository<Training>, TrainingRepository>();
        services.AddScoped<IBaseRepository<MarkValue>, BaseRepository<MarkValue>>();
        services.AddScoped<IBaseRepository<Student>, StudentsRepository>();
    }

    public static void Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapHealthChecks("/_hc");
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
