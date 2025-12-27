using DanplannerBooking.Application.Dtos.Cottage;
using DanplannerBooking.Application.Dtos.Space;
using DanplannerBooking.Application.Dtos.User;
using DanplannerBooking.Domain.Entities;

namespace DanplannerBooking.Application.Dtos.Booking
{
    public class BookingDto
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }

        public Guid? CottageId { get; set; }
        public Guid? SpaceId { get; set; }


    }

    public class BookingCreateCottageDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }

        public Guid? CottageId { get; set; }
        public Guid? SpaceId { get; set; }

        public List<Guid>? AddOnIds { get; set; } // optional
    }

    // READ DTO
    public record BookingResponseDto(
        Guid Id,
        Guid UserId,
        string Name,
        UserResponseDto User,
        int NumberOfPeople,
        DateTime DateStart,
        DateTime DateEnd,
        decimal TotalPrice,
        Guid? CottageId,
        CottageResponseDto Cottage,
        Guid? SpaceId,
        SpaceResponseDto Space,
        List<Guid> AddOnIds
    // Guid? BundleId,
    // Bundle Bundle
    );

    public record BookingDashboardDto(
        Guid Id,
        string Name,
        DateTime DateStart,
        DateTime DateEnd
    );
    public class BookingRangeDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

}
