using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Domain.Entities.JoinTable;
using Microsoft.EntityFrameworkCore;

namespace DanplannerBooking.Infrastructure.Context
{
    public class DbContextBooking : DbContext
    {
        public DbContextBooking(DbContextOptions<DbContextBooking> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Campsite> Campsites { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<Cottage> Cottages { get; set; }
        //public DbSet<Bundle> Bundles { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Layout defaults ---
            modelBuilder.Entity<Space>().Property(x => x.X).HasDefaultValue(0);
            modelBuilder.Entity<Space>().Property(x => x.Y).HasDefaultValue(0);
            modelBuilder.Entity<Cottage>().Property(x => x.X).HasDefaultValue(0);
            modelBuilder.Entity<Cottage>().Property(x => x.Y).HasDefaultValue(0);

            // --- Campsite required columns ---
            modelBuilder.Entity<Campsite>()
                .Property(c => c.Description).HasDefaultValue("");
            modelBuilder.Entity<Campsite>()
                .Property(c => c.Location).HasDefaultValue("N/A");

            // --- Space required columns ---
            modelBuilder.Entity<Space>()
                .Property(s => s.ImageUrl).HasDefaultValue("");
            modelBuilder.Entity<Space>()
                .Property(s => s.PricePerNight).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            // --- Cottage required columns ---
            modelBuilder.Entity<Cottage>()
                .Property(c => c.Description).HasDefaultValue("");
            modelBuilder.Entity<Cottage>()
                .Property(c => c.ImageUrl).HasDefaultValue("");
            modelBuilder.Entity<Cottage>()
                .Property(c => c.Location).HasDefaultValue("N/A");
            modelBuilder.Entity<Cottage>()
                .Property(c => c.PricePerNight).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            // --- Other decimals ---
            modelBuilder.Entity<AddOn>()
                .Property(a => a.Price).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Discount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            modelBuilder.Entity<Bundle>()
                .Property(b => b.BasePrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            modelBuilder.Entity<Bundle>()
                .Property(b => b.Discount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            // ---- Relationships ----

            // Campsite -> Spaces (1:N)
            modelBuilder.Entity<Campsite>()
                .HasMany(c => c.Spaces)
                .WithOne(s => s.Campsite)
                .HasForeignKey(s => s.CampsiteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Campsite -> Cottages (1:N)
            modelBuilder.Entity<Campsite>()
                .HasMany(c => c.Cottages)
                .WithOne(cs => cs.Campsite)
                .HasForeignKey(c => c.CampsiteId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Bookings (1:N)
            modelBuilder.Entity<User>()
                .HasMany<Booking>()
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> BookingAddOn (1:N)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingAddOns)
                .WithOne(ba => ba.Booking)
                .HasForeignKey(ba => ba.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // AddOn -> BookingAddOn (1:N)
            modelBuilder.Entity<AddOn>()
                .HasMany(a => a.BookingAddOns)
                .WithOne(ba => ba.AddOn)
                .HasForeignKey(ba => ba.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bundle -> BundleAddOn (1:N)
            modelBuilder.Entity<Bundle>()
                .HasMany(b => b.BundleAddOns)
                .WithOne(ba => ba.Bundle)
                .HasForeignKey(ba => ba.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            // AddOn -> BundleAddOn (1:N)
            modelBuilder.Entity<AddOn>()
                .HasMany(a => a.BundleAddOns)
                .WithOne(ba => ba.AddOn)
                .HasForeignKey(ba => ba.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Join-table primary keys
            modelBuilder.Entity<BookingAddOn>()
                .HasKey(ba => new { ba.BookingId, ba.AddOnId });

            modelBuilder.Entity<BundleAddOn>()
                .HasKey(ba => new { ba.BundleId, ba.AddOnId });
        }
    }
}
