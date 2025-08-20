# ServiceRegistry

ServiceRegistry provides a simple way for services to register themselves, have their health checked, and be displayed on a dashboard.

## Projects

- **ServiceRegistration.Client** – class library that registers and deregisters a service with the monitor.
- **ServiceRegistry.Monitor** – minimal API that stores registered services in Redis and performs health checks.
- **ServiceRegistry.Dashboard** – Blazor Server dashboard that shows all registered services.

## Prerequisites

- .NET 8 SDK
- Redis running on `localhost:6379`

## Running the Solution

1. Start Redis locally.
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Run the monitor API:
   ```bash
   dotnet run --project src/ServiceRegistry.Monitor
   ```
4. Run the dashboard:
   ```bash
   dotnet run --project src/ServiceRegistry.Dashboard
   ```

Services that use `ServiceRegistration.Client` should call `AddServiceRegistration` and supply the monitor URL and health check information.
