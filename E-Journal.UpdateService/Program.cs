using E_Journal.UpdateService;
using E_Journal.Infrastructure;
using Microsoft.EntityFrameworkCore;

string connectionString = Environment.GetEnvironmentVariable("ConnectionString")
    ?? throw new ArgumentNullException("Environment has not contains the ConnectionString variable that contains database connection string.");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<JournalDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptionsAction: options =>
                {
                    options.EnableRetryOnFailure();
                });
        });
        services.AddHostedService<UpdateService>();
    })
    .Build();

await host.RunAsync();
