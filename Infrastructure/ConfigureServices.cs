using Infrastructure.Redis;
using Interfaces.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
 public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServiceservices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IRedisClient, RedisClient>();

            return services;
        }
    }
}
