using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ServiceRegistration.Client;

public class ServiceRegistrationHostedService : IHostedService
{
    private readonly ServiceRegistrationOptions _options;
    private readonly HttpClient _httpClient = new();

    public ServiceRegistrationHostedService(IOptions<ServiceRegistrationOptions> options)
    {
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var payload = new
        {
            ServiceName = _options.ServiceName,
            InstanceId = _options.InstanceId,
            HealthCheckUrl = _options.HealthCheckUrl
        };
        await _httpClient.PostAsJsonAsync($"{_options.RegistryUrl}/api/registry/register", payload, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var payload = new
        {
            ServiceName = _options.ServiceName,
            InstanceId = _options.InstanceId
        };
        await _httpClient.PostAsJsonAsync($"{_options.RegistryUrl}/api/registry/deregister", payload, cancellationToken);
    }
}
