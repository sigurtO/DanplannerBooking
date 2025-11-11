using DanplannerBooking.Application.Dtos.Campsite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.Cottage
{
    public class CottageDto
    {
        public Guid CampsiteId { get; set; }
        public CampsiteDto? Campsite { get; set; } // Navigation property 
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        // Cottage-specific features
        public bool HasToilet { get; set; }
        public bool HasShower { get; set; }
        public bool HasKitchen { get; set; }
        public bool HasHeating { get; set; }
        public bool HasWiFi { get; set; }

        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }
        public string ImageUrl { get; set; }
    }


    public record CottageResponseDto(
        Guid Id,
        Guid CampsiteId,
        CampsiteResponseDto Campsite,
        string Name,
        string Location,
        string Description,
        bool HasToilet,
        bool HasShower,
        bool HasKitchen,
        bool HasHeating,
        bool HasWiFi,
        bool IsAvailable,
        decimal PricePerNight,
        string ImageUrl
    );
}
