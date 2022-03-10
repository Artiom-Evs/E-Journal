using Microsoft.AspNetCore.Builder;

using E_Journal.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.Server
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

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

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
