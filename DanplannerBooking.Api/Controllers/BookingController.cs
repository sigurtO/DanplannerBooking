using DanplannerBooking.Application.Dtos.Booking;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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
            public string UserName { get; set; } = "";
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
        public async Task<IActionResult> GetBookingsForUnit(Guid unitId, [FromQuery] string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest("Query parameter 'type' must be 'Space' or 'Cottage'.");
            }

            var allBookings = await _bookingRepository.GetAllAsync();

            IEnumerable<Booking> filtered;
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

            var result = filtered.Select(b => new BookingSummaryForUnitDto
            {
                Id = b.Id,
                DateStart = b.DateStart,
                DateEnd = b.DateEnd,
                UserName = b.User?.Name ?? "",
                NumberOfPeople = b.NumberOfPeople,
                TotalPrice = b.TotalPrice
            }).ToList();

            return Ok(result);
        }

        // POST: api/booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
        {
            var newBooking = new Booking
            {
                UserId = bookingDto.UserId,
                NumberOfPeople = bookingDto.NumberOfPeople,
                DateStart = bookingDto.DateStart,
                DateEnd = bookingDto.DateEnd,
                TotalPrice = bookingDto.TotalPrice,
                Discount = bookingDto.Discount,
                CottageId = bookingDto.CottageId,
                SpaceId = bookingDto.SpaceId,
                //BundleId = bookingDto.BundleId
            };

            await _bookingRepository.CreateAsync(newBooking);
            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.Id }, null);
        }

        // PUT: api/booking/{id}
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
                Discount = updatedDto.Discount,
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
    }
}
