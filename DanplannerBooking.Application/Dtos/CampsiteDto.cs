using System;

namespace DanplannerBooking.Application.Dtos
{
    // Simpel DTO til kort/editor: Id, navn og billede
    public sealed class CampsiteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;   // fx "images/hvidbjerg.jpg"
    }
}
