using System;

namespace DanplannerBooking.Application.Dtos
{
    // Bruges til at vise enheder (Space + Cottage) på kort/editor
    public class UnitDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        // "Space" eller "Cottage"
        public string Type { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public Guid CampsiteId { get; set; }
    }

    // Bruges når editoren gemmer layout (kun Id + Type + X + Y)
    public class UnitLayoutDto
    {
        public Guid Id { get; set; }
        // "Space" eller "Cottage"
        public string Type { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
    }
}
