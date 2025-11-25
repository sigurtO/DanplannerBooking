namespace DanplannerBooking.Domain.Entities
{
    public class Space
    {
        public Guid Id { get; set; }
        public Guid CampsiteId { get; set; }

        // Name already existed — keep HEAD version
        public string Name { get; set; } = default!;

        public Campsite Campsite { get; set; } = default!;

        // Layout coordinates
        public int X { get; set; }  // default 0
        public int Y { get; set; }  // default 0

        // Metadata
        public string? Location { get; set; }   // optional
        public string? Description { get; set; } // optional

        public bool HasElectricity { get; set; }
        public int MetersFromToilet { get; set; }

        // Optional distances based on campsite features
        public int? MetersFromPool { get; set; }
        public int? MetersFromPlayground { get; set; }
        public int? MetersFromOcean { get; set; }

        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }

        public byte[] Image { get; set; }
    }
}
