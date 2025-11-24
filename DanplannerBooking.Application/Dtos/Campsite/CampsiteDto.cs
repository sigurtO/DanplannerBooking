using System;
using System.ComponentModel.DataAnnotations;

namespace DanplannerBooking.Application.Dtos.Campsite
{
    // Used for create + update
    public class CreateCampsiteDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public bool HasOceanAccess { get; set; }
        public bool HasPool { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasCarCharger { get; set; }

        // Store image bytes (optional)
        public byte[]? Image { get; set; }
    }

    // Response DTO (adjust as needed)
    public record CampsiteResponseDto(
        Guid Id,
        string Name,
        string Location,
        string Description,
        bool HasOceanAccess,
        bool HasPool,
        bool HasPlayground,
        bool HasCarCharger,
        byte[]? Image
    );
}
