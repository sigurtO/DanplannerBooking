using DanplannerBooking.Domain.Entities.JoinTable;
namespace DanplannerBooking.Domain.Entities
{
    public class Bundle
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; }

        public int? CottageId { get; set; }
        public Cottage Cottage { get; set; }

        public int? SpaceId { get; set; }
        public Space Space { get; set; }

        public ICollection<BundleAddOn> BundleAddOns { get; set; }
    }





}
