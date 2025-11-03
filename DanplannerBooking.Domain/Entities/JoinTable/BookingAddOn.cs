using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Domain.Entities.JoinTable
{
    public class BookingAddOn
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public int AddOnId { get; set; }
        public AddOn AddOn { get; set; }
    }
}
