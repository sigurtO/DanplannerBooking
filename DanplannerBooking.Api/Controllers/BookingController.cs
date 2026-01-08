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
using System.Security.Claims;

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

        // Small DTO for unit list
        public sealed class BookingSummaryForUnitDto
        {
            public Guid Id { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public string Name { get; set; }
            public int NumberOfPeople { get; set; }
            public decimal TotalPrice { get; set; }
        }

        // Small DTO for "my bookings" endpoint
        public sealed class MyBookingDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int NumberOfPeople { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public decimal TotalPrice { get; set; }
            public Guid? CottageId { get; set; }
            public string? CottageName { get; set; }
            public Guid? SpaceId { get; set; }
            public string? SpaceName { get; set; }
        }

        // GET: api/booking
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings);
        }

        // GET: api/booking/my
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var all = await _bookingRepository.GetAllAsync();
            var my = all
                .Where(b => b.UserId == userId)
                .Select(b => new MyBookingDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    NumberOfPeople = b.NumberOfPeople,
                    DateStart = b.DateStart,
                    DateEnd = b.DateEnd,
                    TotalPrice = b.TotalPrice,
                    CottageId = b.CottageId,
                    CottageName = b.Cottage?.Name,
                    SpaceId = b.SpaceId,
                    SpaceName = b.Space?.Name
                })
                .ToList();

            return Ok(my);
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

            var allBookings = (await _bookingRepository.GetAllAsync())?.ToList()
                              ?? new List<Booking>();

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
        [Authorize]
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
            };

            await _bookingRepository.CreateAsync(newBooking);
            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.Id }, null);
        }

        // DELETE: api/booking/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Determine caller identity
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            Guid? callerId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsed))
                callerId = parsed;

            var isAdmin = User.IsInRole("Admin") || (User.FindFirst(ClaimTypes.Role)?.Value == "Admin");

            // Allow deletion if caller is admin or the owner of the booking
            if (!isAdmin && (!callerId.HasValue || booking.UserId != callerId.Value))
            {
                return Forbid();
            }

            var result = await _bookingRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
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
            };

            var updateResult = await _bookingRepository.UpdateAsync(id, updatedBooking);
            if (!updateResult)
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
