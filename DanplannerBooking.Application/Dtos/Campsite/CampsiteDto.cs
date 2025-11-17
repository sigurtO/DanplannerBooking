using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.Campsite
{
    public class CampsiteDto
    {

    }

    public class CreateCampsiteDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        // Shared features
        public bool HasOceanAccess { get; set; }
        public bool HasPool { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasCarCharger { get; set; }
    }

    public record CampsiteResponseDto(
        Guid Id,
        string Name,
        string Location,
        string Description,
        bool HasOceanAccess,
        bool HasPool,
        bool HasPlayground,
        bool HasCarCharger
    );
}
