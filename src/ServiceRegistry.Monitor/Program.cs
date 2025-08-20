using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379"));

var app = builder.Build();

const string HashKey = "services";

app.MapPost("/api/registry/register", async (RegisterRequest req, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var entry = new ServiceEntry(req.ServiceName, req.InstanceId, req.HealthCheckUrl, "Unknown", string.Empty, string.Empty, DateTime.UtcNow);
    await db.HashSetAsync(HashKey, $"{req.ServiceName}:{req.InstanceId}", JsonSerializer.Serialize(entry));
    return Results.Ok();
});

app.MapPost("/api/registry/deregister", async (DeregisterRequest req, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    await db.HashDeleteAsync(HashKey, $"{req.ServiceName}:{req.InstanceId}");
    return Results.Ok();
});

app.MapGet("/api/registry/all", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var entries = new List<ServiceEntry>();
    foreach (var hashEntry in await db.HashGetAllAsync(HashKey))
    {
        var entry = JsonSerializer.Deserialize<ServiceEntry>(hashEntry.Value!)!;
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            var health = await http.GetFromJsonAsync<HealthResponse>(entry.HealthCheckUrl);
            if (health is not null)
            {
                entry = entry with
                {
                    Status = health.status,
                    Version = health.version,
                    Environment = health.environment,
                    LastChecked = DateTime.UtcNow
                };
            }
            else
            {
                entry = entry with { Status = "Unreachable", LastChecked = DateTime.UtcNow };
            }
        }
        catch
        {
            entry = entry with { Status = "Unreachable", LastChecked = DateTime.UtcNow };
        }
        await db.HashSetAsync(HashKey, $"{entry.ServiceName}:{entry.InstanceId}", JsonSerializer.Serialize(entry));
        entries.Add(entry);
    }
    return Results.Ok(entries);
});

app.Run();

record RegisterRequest(string ServiceName, string InstanceId, string HealthCheckUrl);
record DeregisterRequest(string ServiceName, string InstanceId);
record ServiceEntry(string ServiceName, string InstanceId, string HealthCheckUrl, string Status, string Version, string Environment, DateTime LastChecked);
record HealthResponse(string status, string version, string environment);
