using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF; 

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class AlertRepository(PostgreSqlContext postgreSqlContext ) : GenericRepository<Alert>(postgreSqlContext), IAlertRepository
    {
    }
}