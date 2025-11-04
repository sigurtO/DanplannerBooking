namespace DanplannerBooking.Domain.Entities
{
    public class Cottage
    {
        public Guid Id { get; set; }
        public Guid CampsiteId { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; }
        public string Description { get; set; }

        // NEW: editor layout coords (SVG pixel space)
        public int X { get; set; }  // default 0
        public int Y { get; set; }  // default 0

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





}
