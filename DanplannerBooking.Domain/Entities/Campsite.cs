using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Domain.Entities
{
    public class Campsite
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        // Shared features
        public bool HasOceanAccess { get; set; }
        public bool HasPool { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasCarCharger { get; set; }//not used

        public ICollection<Space> Spaces { get; set; } // one to many relationship
        public ICollection<Cottage> Cottages { get; set; } // one to many relationship
    }





}
