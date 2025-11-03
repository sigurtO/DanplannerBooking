using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Domain.Entities.JoinTable;
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


        public DbSet<User> Users { get; set; }
        public DbSet<Campsite> Campsites { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<Cottage> Cottages { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<Booking> Bookings { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Campsite -> Spaces (One-to-Many)
            modelBuilder.Entity<Campsite>()
                .HasMany(c => c.Spaces)
                .WithOne(s => s.Campsite)
                .HasForeignKey(s => s.CampsiteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Campsite -> Cottages (One-to-Many)
            modelBuilder.Entity<Campsite>()
                .HasMany(c => c.Cottages)
                .WithOne() // Cottage doesn't have navigation back to Campsite
                .HasForeignKey(c => c.CampsiteId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Bookings (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany<Booking>() // User can have many bookings
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> BookingAddOn (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingAddOns)
                .WithOne(ba => ba.Booking)
                .HasForeignKey(ba => ba.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // AddOn -> BookingAddOn (One-to-Many)
            modelBuilder.Entity<AddOn>()
                .HasMany(a => a.BookingAddOns)
                .WithOne(ba => ba.AddOn)
                .HasForeignKey(ba => ba.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bundle -> BundleAddOn (One-to-Many)
            modelBuilder.Entity<Bundle>()
                .HasMany(b => b.BundleAddOns)
                .WithOne(ba => ba.Bundle)
                .HasForeignKey(ba => ba.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            // AddOn -> BundleAddOn (One-to-Many)
            modelBuilder.Entity<AddOn>()
                .HasMany(a => a.BundleAddOns)
                .WithOne(ba => ba.AddOn)
                .HasForeignKey(ba => ba.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure composite keys for join tables 
            modelBuilder.Entity<BookingAddOn>()
                .HasKey(ba => new { ba.BookingId, ba.AddOnId });

            modelBuilder.Entity<BundleAddOn>()
                .HasKey(ba => new { ba.BundleId, ba.AddOnId });
        }



    }
}
