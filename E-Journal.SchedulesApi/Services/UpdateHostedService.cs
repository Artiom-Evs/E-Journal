namespace E_Journal.SchedulesApi.Services;

public class UpdateHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateHostedService> _logger;
    private readonly IWebAccessorService _webAccessor;
    private readonly IParserService _parser;
    
    private IUpdateService _updater;
    private ApplicationDbContext _context;

    public UpdateHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<UpdateHostedService> logger, IWebAccessorService webAccessor, IParserService parser)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
        _webAccessor = webAccessor;
        _parser = parser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        _updater = scope.ServiceProvider.GetRequiredService<IUpdateService>();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int checkFrequency;
        string url = _configuration["GROUP_WEEKLY_SCHEDULE_URL"];
        
        if (!int.TryParse(_configuration["UPDATE_CHECK_FREQUENCY_MINUTES"], out checkFrequency))
        {
            throw new InvalidOperationException("ENVIRONMENT do not contasin 'UPDATE_CHECK_FREQUENCY_MINUTES' variable.");
        }

        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("ENVIRONMENT do not contasin 'GROUP_WEEKLY_SCHEDULE_URL' variable.");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(">> Update of the weekly schedules has started");
            await UpdateWeeklySchedulesAsync(url, stoppingToken);
            _logger.LogInformation(">> Update of the weekly schedules has ended");

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

    private async Task<bool> UpdateWeeklySchedulesAsync(string url, CancellationToken cancellationToken)
    {
        string? pageText = await _webAccessor.GetWebPageAsync(url, cancellationToken);

        if (pageText == null)
        {
            return false;
        }

        var parsedLessons = _parser.ParseWeeklySchedulesPage(pageText);

        return await _updater.UpdateAsync(parsedLessons);
    }
}
