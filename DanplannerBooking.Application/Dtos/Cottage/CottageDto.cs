using DanplannerBooking.Application.Dtos.Campsite;
using System;
using System.ComponentModel.DataAnnotations;

namespace DanplannerBooking.Application.Dtos.Cottage
{
    public class CottageDto
    {
        [Required]
        public Guid CampsiteId { get; set; }

        public CampsiteDto? Campsite { get; set; } // Navigation property

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Cottage-specific features
        public bool HasToilet { get; set; }
        public bool HasShower { get; set; }
        public bool HasKitchen { get; set; }
        public bool HasHeating { get; set; }
        public bool HasWiFi { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, 100000, ErrorMessage = "Price per night must be greater than 0.")]
        public decimal PricePerNight { get; set; }

        public byte[] Image { get; set; } = Array.Empty<byte>();
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
        byte[] Image
    );
}
