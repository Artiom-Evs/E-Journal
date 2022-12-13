namespace E_Journal.SchedulesApi.Services;

public class UpdateHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateHostedService> _logger;
    
    private IWebAccessorService _webAccessor;
    private IParserService _parser;
    private IUpdateService _updater;

    private ApplicationDbContext _context;

    public UpdateHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<UpdateHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        _webAccessor = scope.ServiceProvider.GetRequiredService<IWebAccessorService>();
        _parser = scope.ServiceProvider.GetRequiredService<IParserService>();
        _updater = scope.ServiceProvider.GetRequiredService<IUpdateService>();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int checkFrequency;
        string uri1 = _configuration["BUILDING_1_GROUP_DAYLY_SCHEDULE_URL"];
        string uri2 = _configuration["BUILDING_2_GROUP_DAYLY_SCHEDULE_URL"];

        if (!int.TryParse(_configuration["UPDATE_CHECK_FREQUENCY_MINUTES"], out checkFrequency))
        {
            throw new InvalidOperationException("ENVIRONMENT do not contasin 'UPDATE_CHECK_FREQUENCY_MINUTES' variable.");
        }

        if (string.IsNullOrEmpty(uri1))
        {
            throw new InvalidOperationException("ENVIRONMENT do not contasin 'BUILDING_1_GROUP_DAYLY_SCHEDULE_URL' variable.");
        }

        if (string.IsNullOrEmpty(uri2))
        {
            throw new InvalidOperationException("ENVIRONMENT do not contasin 'BUILDING_2_GROUP_DAYLY_SCHEDULE_URL' variable.");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(">> Update of the dayly schedules of the building 1 has started");
            await UpdateDaylySchedulesAsync(uri1, stoppingToken);
            _logger.LogInformation(">> Update of the dayly schedules of the building 1 has ended");

            _logger.LogInformation(">> Update of the dayly schedules of the building 2 has started");
            await UpdateDaylySchedulesAsync(uri2, stoppingToken);
            _logger.LogInformation(">> Update of the dayly schedules of the building 2 has ended");

            await Task.Delay(TimeSpan.FromMinutes(checkFrequency), stoppingToken);
        }
    }

    private async Task<bool> UpdateDaylySchedulesAsync(string url, CancellationToken cancellationToken)
    {
        string? pageText = await _webAccessor.GetWebPageAsync(url, cancellationToken);

        if (pageText == null)
        {
            return false;
        }

        var parsedLessons = _parser.ParseDaylySchedulesPage(pageText);
        return await _updater.UpdateAsync(parsedLessons);
    }
}
