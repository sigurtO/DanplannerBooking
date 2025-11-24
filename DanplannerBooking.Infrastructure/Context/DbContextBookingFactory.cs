using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DanplannerBooking.Infrastructure.Context
{
    // Bruges KUN af EF Core ved design-time (Add-Migration / Update-Database)
    public class DbContextBookingFactory : IDesignTimeDbContextFactory<DbContextBooking>
    {
        public DbContextBooking CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextBooking>();

            
            var connectionString =
                "Server=localhost,1435;Database=DanplannerBookingDockerDb;User Id=sa;Password=MySqlServer123!;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new DbContextBooking(optionsBuilder.Options);
        }
    }
}
