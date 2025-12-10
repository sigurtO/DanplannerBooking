using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DanplannerBooking.UnitTests;

public class TestBase
{
    [Fact]
    public async Task UpdateAsync_ShouldThrowConcurrencyException_WhenRowVersionMismatch()
    {
        var options = new DbContextOptionsBuilder<DbContextBooking>()
            .UseSqlServer("Server=localhost,1435;Database=DanplannerBookingDockerDb;User Id=sa;Password=MySqlServer123!;TrustServerCertificate=True;")
            .Options;

        Guid cottageId;

        // Arrange: Opret en campsite + cottage og gem dem rigtigt
        using (var context = new DbContextBooking(options))
        {
            var campsite = new Campsite
            {
                Id = Guid.NewGuid(),
                Name = "Test Campsite",
                Location = "Test Location",
                Image = Array.Empty<byte>()
            };
            context.Campsites.Add(campsite);

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

            cottageId = cottage.Id;
        }

        Cottage cottage1;
        Cottage cottage2;

        // Load samme entity i to forskellige contexts
        using (var context = new DbContextBooking(options))
        {
            cottage1 = await context.Cottages.SingleAsync(c => c.Id == cottageId);
        }

        using (var context = new DbContextBooking(options))
        {
            cottage2 = await context.Cottages.SingleAsync(c => c.Id == cottageId);
        }

        // User 1: opdaterer og gemmer
        using (var context = new DbContextBooking(options))
        {
            cottage1.Name = "Updated Cottage";
            context.Cottages.Update(cottage1);
            await context.SaveChangesAsync();
        }

        // User 2: forsøger at gemme gammel version → forvent concurrency exception
        using (var context = new DbContextBooking(options))
        {
            cottage2.Name = "Another Update";
            context.Cottages.Update(cottage2);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                context.SaveChangesAsync());
        }

        // Cleanup: slet test-data
        using (var context = new DbContextBooking(options))
        {
            var toDelete = await context.Cottages.SingleAsync(c => c.Id == cottageId);
            context.Cottages.Remove(toDelete);

            // evt. også slette campsite hvis der ikke er andre cottages på den
            var campsite = await context.Campsites.SingleAsync(c => c.Id == toDelete.CampsiteId);
            context.Campsites.Remove(campsite);

            await context.SaveChangesAsync();
        }
    }
}
