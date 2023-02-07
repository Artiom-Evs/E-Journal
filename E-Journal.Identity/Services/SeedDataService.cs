using E_Journal.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;

namespace E_Journal.Identity.Services;

public class SeedDataService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SeedDataService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        await PopulateIdentityRolesAsync(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task PopulateIdentityRolesAsync(IServiceScope scope)
    {
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = Roles.GetAll();
        
        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
