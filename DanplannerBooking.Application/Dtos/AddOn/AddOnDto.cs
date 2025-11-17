using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.AddOn
{
    public class AddOnDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public AddOnType Type { get; set; }
    }

    public record AddOnResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        AddOnType Type
    );


    public enum AddOnTypeDto //for testing might move it somewhere else this is def on clean arch 
    {
        Pet,           // Dog, Cat, etc.
        Cleaning,       // Room cleaning service
        Bubbles,       // Champagne on arrival
        Equipment      // Extra beds, chairs, etc.
    }
}
