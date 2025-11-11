namespace DanplannerBooking.Domain.Entities
{
    public class Cottage
    {
        public Guid Id { get; set; }
        public Guid CampsiteId { get; set; }
        public Campsite Campsite { get; set; } // Navigation property
        public string Name { get; set; }
        public string Location { get; set; } //area
        public string Description { get; set; }

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
