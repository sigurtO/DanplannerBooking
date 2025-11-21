using DanplannerBooking.Application.Dtos.Booking;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateCottageDto bookingDto)
        {
            var newBooking = new Booking
            {
                UserId = bookingDto.UserId,
                NumberOfPeople = bookingDto.NumberOfPeople,
                DateStart = bookingDto.DateStart,
                DateEnd = bookingDto.DateEnd,
                CottageId = bookingDto.CottageId,
                SpaceId = bookingDto.SpaceId,
                //BundleId = bookingDto.BundleId
                // BookingAddOns should be handled separately if needed
            };

            await _bookingRepository.CreateAsync(newBooking);
            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingDto updatedDto)
        {
            var updatedBooking = new Booking
            {
                UserId = updatedDto.UserId,
                NumberOfPeople = updatedDto.NumberOfPeople,
                DateStart = updatedDto.DateStart,
                DateEnd = updatedDto.DateEnd,
                TotalPrice = updatedDto.TotalPrice,
                CottageId = updatedDto.CottageId,
                SpaceId = updatedDto.SpaceId,
                //BundleId = updatedDto.BundleId
            };

            var result = await _bookingRepository.UpdateAsync(id, updatedBooking);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var result = await _bookingRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<BookingDashboardDto>>> GetDashboardData()
        {
            var data = await _bookingRepository.GetDashboardDataAsync();
            return Ok(data);
        }

        [HttpGet("arrivals-today")]
        public async Task<ActionResult<IEnumerable<BookingDashboardDto>>> GetArrivalsToday()
        {
            var today = DateTime.Today;

            var data = await _bookingRepository.GetDashboardDataAsync();

            return Ok(data.Where(b => b.DateStart.Date == today));
        }

        [HttpGet("departures-today")]
        public async Task<ActionResult<IEnumerable<BookingDashboardDto>>> GetDeparturesToday()
        {
            var today = DateTime.Today;

            var data = await _bookingRepository.GetDashboardDataAsync();

            return Ok(data.Where(b => b.DateEnd.Date == today));
        }




    }
}
