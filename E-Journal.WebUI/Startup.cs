using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

using E_Journal.Infrastructure;
using E_Journal.Shared;

namespace E_Journal.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddDbContext<JournalDbContext>(options =>
            {
                string connectionString = Configuration["ConnectionString"];
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptionsAction: options =>
                    {
                        options.EnableRetryOnFailure();
                    });
            });
            services.AddScoped<IJournalRepository, JournalRepository>();
        }
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
