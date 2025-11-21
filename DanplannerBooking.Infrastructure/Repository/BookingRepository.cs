using DanplannerBooking.Application.Dtos.Booking;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DbContextBooking _context;

        public BookingRepository(DbContextBooking context)
        {
            _context = context;
        }

        public async Task CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Cottage)
                .Include(b => b.Space)
                //.Include(b => b.Bundle)
                .Include(b => b.BookingAddOns)
                .ToListAsync();
        }

        public async Task<Booking> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Cottage)
                .Include(b => b.Space)
                //.Include(b => b.Bundle)
                .Include(b => b.BookingAddOns)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> UpdateAsync(Guid id, Booking updatedBooking)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null) return false;

            existingBooking.UserId = updatedBooking.UserId;
            existingBooking.NumberOfPeople = updatedBooking.NumberOfPeople;
            existingBooking.DateStart = updatedBooking.DateStart;
            existingBooking.DateEnd = updatedBooking.DateEnd;
            existingBooking.TotalPrice = updatedBooking.TotalPrice;
            existingBooking.CottageId = updatedBooking.CottageId;
            existingBooking.SpaceId = updatedBooking.SpaceId;
            //existingBooking.BundleId = updatedBooking.BundleId;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<BookingDashboardDto>> GetDashboardDataAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Select(b => new BookingDashboardDto(
                    b.Id,
                    b.User.Name,
                    b.DateStart,
                    b.DateEnd
                ))
                .ToListAsync();
        }

    }

}
