using DanplannerBooking.Application.Dtos.Booking;
using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(Guid id);
        Task CreateAsync(Booking booking);
        Task<bool> UpdateAsync(Guid id, Booking booking);
        Task<bool> DeleteAsync(Guid id);
        Task<List<BookingDashboardDto>> GetDashboardDataAsync();

    }
}
