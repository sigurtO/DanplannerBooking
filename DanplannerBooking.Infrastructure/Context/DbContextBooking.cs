using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Infrastructure.Context
{
    public class DbContextBooking : DbContext
    {
        public DbContextBooking(DbContextOptions<DbContextBooking> options) : base(options)
        {

        }


        public DbSet<BookingAddOn> BookingAddOns { get; set; }
        public DbSet<BundleAddOn> BundleAddOns { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
        }



    }
}
