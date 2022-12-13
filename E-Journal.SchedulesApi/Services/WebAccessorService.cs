namespace E_Journal.SchedulesApi.Services;

public class WebAccessorService : IWebAccessorService, IDisposable
{
    private readonly ILogger<WebAccessorService> _logger;
    private readonly HttpClient _client;
    private bool disposedValue;

    public WebAccessorService(ILogger<WebAccessorService> logger)
    {
        _logger = logger;
        _client = new HttpClient();
    }

    public async Task<string?> GetWebPageAsync(string uri, CancellationToken? token = null)
    {
        CancellationToken cancellationToken = token ?? new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

        try
        {
            Uri newUri = new(uri);
            string pageText = await _client.GetStringAsync(newUri, cancellationToken);
            return pageText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured while getting web-page");
            return null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
