using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos
{
    // For GET /api/units (viewer/editor)
    public sealed class UnitDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        // "Space" or "Cottage" (UI can map to "Plads"/"Hytte" if needed)
        public string Type { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
    }

    // For PUT /api/units/layout (save positions)
    public sealed class UnitLayoutDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = ""; // "Space" or "Cottage"
        public int X { get; set; }
        public int Y { get; set; }
    }
}
