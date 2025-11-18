using DanplannerBooking.Application.Dtos.Cottage;
using DanplannerBooking.Application.Dtos.Space;
using DanplannerBooking.Application.Dtos.User;
using DanplannerBooking.Domain.Entities;

namespace DanplannerBooking.Application.Dtos.Booking
{
    public class BookingDto
    {
        public Guid UserId { get; set; }

        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }

        public Guid? CottageId { get; set; }
        public Guid? SpaceId { get; set; }

        // VIGTIGT: ingen navigation properties her!
        // public UserResponseDto User { get; set; }
        // public CottageResponseDto Cottage { get; set; }
        // public SpaceResponseDto Space { get; set; }
        // public Guid? BundleId { get; set; }
        // public Bundle Bundle { get; set; }
    }

    // Denne kan du godt beholde som din "read" DTO:
    public record BookingResponseDto(
        Guid Id,
        Guid UserId,
        UserResponseDto User,
        int NumberOfPeople,
        DateTime DateStart,
        DateTime DateEnd,
        decimal TotalPrice,
        decimal Discount,
        Guid? CottageId,
        CottageResponseDto Cottage,
        Guid? SpaceId,
        SpaceResponseDto Space
    //Guid? BundleId,
    //Bundle Bundle
    );
}
