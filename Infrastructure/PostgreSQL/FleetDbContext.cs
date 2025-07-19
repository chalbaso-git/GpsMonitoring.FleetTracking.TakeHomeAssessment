using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL
{
    public class FleetDbContext : DbContext
    {
        public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        // Agrega otros DbSet según tus tablas
    }
}