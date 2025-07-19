using Infrastructure.PostgreSQL;
using Infrastructure.Redis;
using Interfaces.Infrastructure;
using Interfaces.Infrastructure.EF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
 public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServiceservices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IRedisClient, RedisClient>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            return services;
        }
    }
}
