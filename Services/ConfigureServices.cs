using Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementations;

namespace Services
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ICircuitBreakerService, CircuitBreakerService>();
            services.AddScoped<IGeolocationService, GeolocationService>(); 
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRoutingService, RoutingService>();
            services.AddScoped<IVehicleService, VehicleService>();

            return services;
        }
    }
}
