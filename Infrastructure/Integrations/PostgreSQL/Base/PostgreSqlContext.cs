using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Integrations.PostgreSQL.Base
{
    public class PostgreSqlContext : DbContext
    {
        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Waypoint> Waypoints { get; set; }
    }
}