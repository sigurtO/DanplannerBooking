using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.UnitTests;

public class TestBase
{
    [Fact]
    public async Task UpdateAsync_ShouldThrowConcurrencyException_WhenRowVersionMismatch()
    {
        var options = new DbContextOptionsBuilder<DbContextBooking>()
            .UseSqlServer("Server=localhost,1435;Database=DanplannerBookingDockerDb;User Id=sa;Password=MySqlServer123!;TrustServerCertificate=True;")
            .Options;
        //Arange: create and save a new entity
        Guid campsiteId;
        Guid cottageId;

        using (var context = new DbContextBooking(options))
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            var campsite = new Campsite
            {
                Id = Guid.NewGuid(),
                Name = "Test Campsite",
                Location = "Test Location",
                Image = Array.Empty<byte>()
            };
            context.Campsites.Add(campsite);
            await context.SaveChangesAsync();

            var cottage = new Cottage
            {
                Id = Guid.NewGuid(),
                CampsiteId = campsite.Id,
                Name = "Test Cottage",
                Location = "Lake",
                PricePerNight = 100,
                Image = Array.Empty<byte>()
            };
            context.Cottages.Add(cottage);
            await context.SaveChangesAsync();

            campsiteId = campsite.Id;
            cottageId = cottage.Id;

            //så vi sletter de fake cottages og campsite efter testen    
            await transaction.RollbackAsync();
        }

        // Act: load the same entity twice (using fresh contexts)
        Cottage cottage1, cottage2;
        using (var context = new DbContextBooking(options))
        {
            cottage1 = await context.Cottages.FirstAsync();
        }
        using (var context = new DbContextBooking(options))
        {
            cottage2 = await context.Cottages.FirstAsync();
        }

        // User 1Update and saves
        using (var context = new DbContextBooking(options))
        {
            cottage1.Name = "Updated Cottage";
            context.Cottages.Update(cottage1);
            await context.SaveChangesAsync();
        }

        // User 2 tries to update and save should cause concurrency exception
        using (var context = new DbContextBooking(options))
        {
            cottage2.Name = "Another Update";
            context.Cottages.Update(cottage2);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                context.SaveChangesAsync());
        }
    }

}

