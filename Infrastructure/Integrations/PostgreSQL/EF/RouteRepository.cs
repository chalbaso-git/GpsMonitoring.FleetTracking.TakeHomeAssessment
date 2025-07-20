using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class RouteRepository(PostgreSqlContext postgreSqlContext) : GenericRepository<Route>(postgreSqlContext), IRouteRepository
    {
    }
}