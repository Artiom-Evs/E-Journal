using System.Runtime.CompilerServices;

namespace E_Journal.SchedulesApi.Services;

public interface IWebAccessorService
{
    Task<string?> GetWebPageAsync(string uri, CancellationToken? token = null);
}
