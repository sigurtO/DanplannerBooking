namespace DanplannerBooking.Domain.Entities
{
    public class Booking
    {
        public int Guid { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }

        public int? CottageId { get; set; }
        public Cottage Cottage { get; set; }

        public int? SpaceId { get; set; }
        public Space Space { get; set; }

        public int? BundleId { get; set; }
        public Bundle Bundle { get; set; }

        public ICollection<BookingAddOn> BookingAddOns { get; set; }
    }

}
