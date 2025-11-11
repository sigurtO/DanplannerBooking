using DanplannerBooking.Application.Dtos.Campsite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.Space
{
    public class CreateSpaceDto
    {
        public Guid CampsiteId { get; set; }
        public CampsiteResponseDto? Campsite { get; set; }

        public string Name { get; set; } // new
        public string Location { get; set; } // new
        public string Description { get; set; } // new

        public bool HasElectricity { get; set; }
        public int MetersFromToilet { get; set; }

        // Optional distances based on campsite features
        public int? MetersFromPool { get; set; }
        public int? MetersFromPlayground { get; set; }
        public int? MetersFromOcean { get; set; }

        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }
        public string ImageUrl { get; set; }
    }

    public record SpaceResponseDto(
        Guid Id,
        Guid CampsiteId,
        CampsiteResponseDto Campsite,
        string Name,
        string Location,
        string Description,
        bool HasElectricity,
        int MetersFromToilet,
        int? MetersFromPool,
        int? MetersFromPlayground,
        int? MetersFromOcean,
        bool IsAvailable,
        decimal PricePerNight,
        string ImageUrl
    );
}
