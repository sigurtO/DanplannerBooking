using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos
{
    public sealed class ImportUnitDto
    {
        public Guid? Id { get; set; }            // optional; if present we use it
        public string Name { get; set; } = "";   // used if Id missing
        public string Type { get; set; } = "";   // "Space" or "Cottage"
        public int X { get; set; }
        public int Y { get; set; }
    }

    // request envelope (lets us add flags)
    public sealed class ImportMapRequest
    {
        public List<ImportUnitDto> Units { get; set; } = new();
        public bool CreateMissing { get; set; } = false; // if true, create new Space/Cottage if not found
    }
}
