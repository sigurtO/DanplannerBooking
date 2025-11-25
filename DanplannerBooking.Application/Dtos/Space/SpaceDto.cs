using DanplannerBooking.Application.Dtos.Campsite;
using System;
using System.ComponentModel.DataAnnotations;

namespace DanplannerBooking.Application.Dtos.Space
{
    public class CreateSpaceDto
    {
        [Required]
        public Guid CampsiteId { get; set; }
        public CampsiteResponseDto? Campsite { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool HasElectricity { get; set; }

        [Range(0, 10000)]
        public int MetersFromToilet { get; set; }

        // Optional distances based on campsite features
        [Range(0, 10000)]
        public int? MetersFromPool { get; set; }

        [Range(0, 10000)]
        public int? MetersFromPlayground { get; set; }

        [Range(0, 10000)]
        public int? MetersFromOcean { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, 100000, ErrorMessage = "Price per night must be greater than 0.")]
        public decimal PricePerNight { get; set; }

        public byte[] Image { get; set; }
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
        byte[] Image
    );
}
