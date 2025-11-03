using DanplannerBooking.Domain.Entities.JoinTable;
namespace DanplannerBooking.Domain.Entities
{
    public class AddOn
    {
        public Guid Id { get; set; }
        public string Name { get; set; }  // e.g., "Dog", "Cleaning", "Champagne"
        public string Description { get; set; }
        public decimal Price { get; set; }

        public ICollection<BookingAddOn> BookingAddOns { get; set; }
        public ICollection<BundleAddOn> BundleAddOns { get; set; }
    }





}
