using DanplannerBooking.Domain.Entities.JoinTable;
namespace DanplannerBooking.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Name { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal TotalPrice { get; set; }
       // public decimal Discount { get; set; } //remove this????

        public Guid? CottageId { get; set; }
        public Cottage Cottage { get; set; }

        public Guid? SpaceId { get; set; }
        public Space Space { get; set; }

        //public Guid? BundleId { get; set; }
        //public Bundle Bundle { get; set; }

        public ICollection<BookingAddOn> BookingAddOns { get; set; }
    }

}
