using DanplannerBooking.Application.Dtos.Cottage;
using DanplannerBooking.Application.Dtos.Space;
using DanplannerBooking.Application.Dtos.User;
using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.Booking
{
    public class BookingDto
    {
        public Guid UserId { get; set; }
        public UserResponseDto User { get; set; }

        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }

        public Guid? CottageId { get; set; }
        public CottageResponseDto Cottage { get; set; }

        public Guid? SpaceId { get; set; }
        public SpaceResponseDto Space { get; set; }

        //public Guid? BundleId { get; set; }
        //public Bundle Bundle { get; set; }

    }

    public class BookingCreateCottageDto
    {
        public Guid UserId { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public Guid? CottageId { get; set; }
        public Guid? SpaceId { get; set; }

        public List<Guid>? AddOnIds { get; set; } // optional
    }
    public record BookingResponseDto(
        Guid Id,
        Guid UserId,
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

    //Guid? BundleId,
    //Bundle Bundle
    );
    public record BookingDashboardDto(
        Guid Id,
        string UserName,
        DateTime DateStart,
        DateTime DateEnd
    );
}
