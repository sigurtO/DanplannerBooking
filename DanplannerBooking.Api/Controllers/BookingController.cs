using DanplannerBooking.Application.Dtos.Booking;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // --------- DTO til map-viewet ----------
        public sealed class BookingSummaryForUnitDto
        {
            public Guid Id { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public string Name { get; set; }
            public int NumberOfPeople { get; set; }
            public decimal TotalPrice { get; set; }
        }

        // GET: api/booking
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings);
        }

        // GET: api/booking/{id}
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

        // GET: api/booking/by-unit/{unitId}?type=Space|Cottage
        [HttpGet("by-unit/{unitId:guid}")]
        public async Task<ActionResult<IEnumerable<BookingSummaryForUnitDto>>> GetBookingsForUnit(
            Guid unitId,
            [FromQuery] string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest("Query parameter 'type' must be 'Space' or 'Cottage'.");
            }

            // Hent alle bookinger (inkl. navigation properties) og sørg for,
            // at vi ikke crasher, selv hvis repositoriet skulle returnere null.
            var allBookings = (await _bookingRepository.GetAllAsync())?.ToList()
                              ?? new List<Booking>();

            // Hvis der slet ikke er bookinger, returnér bare en tom liste.
            if (!allBookings.Any())
            {
                return Ok(new List<BookingSummaryForUnitDto>());
            }

            IEnumerable<Booking>? filtered = null;

            if (type.Equals("Space", StringComparison.OrdinalIgnoreCase))
            {
                filtered = allBookings.Where(b => b.SpaceId == unitId);
            }
            else if (type.Equals("Cottage", StringComparison.OrdinalIgnoreCase))
            {
                filtered = allBookings.Where(b => b.CottageId == unitId);
            }
            else
            {
                return BadRequest("Query parameter 'type' must be 'Space' or 'Cottage'.");
            }

            var result = filtered
                .Select(b => new BookingSummaryForUnitDto
                {
                    Id = b.Id,
                    DateStart = b.DateStart,
                    DateEnd = b.DateEnd,
                    Name = b.Name,
                    NumberOfPeople = b.NumberOfPeople,
                    TotalPrice = b.TotalPrice
                })
                .ToList();

            return Ok(result);
        }

        // POST: api/booking
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateCottageDto bookingDto)
        {
            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = bookingDto.UserId,
                Name = bookingDto.Name,
                NumberOfPeople = bookingDto.NumberOfPeople,
                DateStart = bookingDto.DateStart,
                DateEnd = bookingDto.DateEnd,
                CottageId = bookingDto.CottageId,
                SpaceId = bookingDto.SpaceId,
                //BundleId = bookingDto.BundleId
            };

            await _bookingRepository.CreateAsync(newBooking);
            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.Id }, null);
        }

        // PUT: api/booking/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingDto updatedDto)
        {
            var updatedBooking = new Booking
            {
                UserId = updatedDto.UserId,
                Name = updatedDto.Name,
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

        // DELETE: api/booking/{id}
        [Authorize(Roles = "Admin")]
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


        [HttpGet("unavailable/{cottageId}")]
        public async Task<IActionResult> GetUnavailableDates(Guid cottageId)
        {
            var ranges = await _bookingRepository.GetBookedDateRangesForCottageAsync(cottageId);
            return Ok(ranges);
        }

        // GET: api/booking/unavailable-space/{spaceId}
        [HttpGet("unavailable-space/{spaceId}")]
        public async Task<IActionResult> GetUnavailableDatesForSpace(Guid spaceId)
        {
            var ranges = await _bookingRepository.GetBookedDateRangesForSpaceAsync(spaceId);
            return Ok(ranges);
        }





    }
}
