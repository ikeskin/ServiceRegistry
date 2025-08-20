namespace ServiceRegistration.Client;

public class ServiceRegistrationOptions
{
    public required string ServiceName { get; init; }
    public required string InstanceId { get; init; }
    public required string HealthCheckUrl { get; init; }
    public required string RegistryUrl { get; init; }
}
