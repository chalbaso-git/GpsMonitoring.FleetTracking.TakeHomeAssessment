using Infrastructure.Integrations.PostgreSQL.Base;
using Infrastructure.Integrations.PostgreSQL.EF;
using Infrastructure.Integrations.Redis;
using Interfaces.Infrastructure;
using Interfaces.Infrastructure.EF;
using Interfaces.Infrastructure.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
 public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServiceservices(this IServiceCollection services, IConfiguration config)
        {
            string? connectionString = config["ConnectionStrings:PostgreSQL"];

            services.AddDbContext<PostgreSqlContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(30);
                               npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3, 
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                // Pool de conexiones recomendado para robustez
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            services.AddScoped<IRedisClient, RedisClient>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();
            services.AddScoped<IRouteCache, RouteCache>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IWaypointRepository, WaypointRepository>();

            return services;
        }
    }
}
