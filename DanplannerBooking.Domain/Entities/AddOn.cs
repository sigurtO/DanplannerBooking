using DanplannerBooking.Domain.Entities.JoinTable;

namespace DanplannerBooking.Domain.Entities
{
    public class AddOn
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;        // fx "Dog", "Cleaning", "Champagne"
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public AddOnType Type { get; set; }
        public bool IsActive { get; set; } = true;          // Soft delete

        public ICollection<BookingAddOn> BookingAddOns { get; set; } = new List<BookingAddOn>();
        public ICollection<BundleAddOn> BundleAddOns { get; set; } = new List<BundleAddOn>();
    }

    public enum AddOnType
    {
        Pet,       // Dog, Cat, etc.
        Cleaning,  // Room cleaning service
        Bubbles,   // Champagne on arrival
        Equipment  // Extra beds, chairs, etc.
    }
}
