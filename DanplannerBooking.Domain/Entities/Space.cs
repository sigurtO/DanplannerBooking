namespace DanplannerBooking.Domain.Entities
{
    public class Space //add name to space GODDAMNIT
    {
        public Guid Id { get; set; }
        public Guid CampsiteId { get; set; }
        public Campsite Campsite { get; set; }

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
        public byte[] Image { get; set; }
    }





}
