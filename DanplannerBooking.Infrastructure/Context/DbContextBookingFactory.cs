using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DanplannerBooking.Infrastructure.Context
{
    // Bruges KUN af EF Core ved design-time (Add-Migration / Update-Database)
    public class DbContextBookingFactory : IDesignTimeDbContextFactory<DbContextBooking>
    {
        public DbContextBooking CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextBooking>();

            // Locate the API project's appsettings.json at design time
            // Assumes solution structure: Infrastructure and Api at same root
            var basePath = Directory.GetCurrentDirectory();
            var apiProjectPath = Path.Combine(basePath, "..", "DanplannerBooking.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new DbContextBooking(optionsBuilder.Options);
        }
    }
}
