using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL
{
    public class FleetDbContext : DbContext
    {
        public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; }
        // Puedes agregar otros DbSet aquí (por ejemplo, vehículos, rutas, etc.)
    }
}