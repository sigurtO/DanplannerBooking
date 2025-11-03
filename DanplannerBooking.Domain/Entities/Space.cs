namespace DanplannerBooking.Domain.Entities
{
    public class Space
    {
        public Guid Id { get; set; }
        public int CampsiteId { get; set; }
        public Campsite Campsite { get; set; }

        public bool HasElectricity { get; set; }
        public int MetersFromToilet { get; set; }

        // Optional distances based on campsite features
        public int? MetersFromPool { get; set; }
        public int? MetersFromPlayground { get; set; }
        public int? MetersFromOcean { get; set; }

        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }





}
