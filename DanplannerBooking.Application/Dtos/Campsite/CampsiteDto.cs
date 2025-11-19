using System;
using System.ComponentModel.DataAnnotations;

namespace DanplannerBooking.Application.Dtos.Campsite
{
    // Bruges til at oprette/ændre en campsite (Create + Update)
    public class CreateCampsiteDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool HasOceanAccess { get; set; }
        public bool HasPool { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasCarCharger { get; set; }
    }

    // Bruges til detaljer/oversigts-sider (CreateCottage, CreateSpace, /campsites)
    public record CampsiteResponseDto(
        Guid Id,
        string Name,
        string Location,
        string Description,
        bool HasOceanAccess,
        bool HasPool,
        bool HasPlayground,
        bool HasCarCharger
    );
}
