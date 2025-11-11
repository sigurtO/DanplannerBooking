using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Infrastructure.Repository
{
    public class AddOnRepository : IAddOnRepository
    {
        private readonly DbContextBooking _context;

        public AddOnRepository(DbContextBooking context)
        {
            _context = context;
        }

        public async Task CreateAsync(AddOn addOn)
        {
            _context.AddOns.Add(addOn);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AddOn>> GetAllAsync()
        {
            return await _context.AddOns.ToListAsync();
        }

        public async Task<AddOn> GetByIdAsync(Guid id)
        {
            return await _context.AddOns.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Guid id, AddOn updatedAddOn)
        {
            var existingAddOn = await _context.AddOns.FindAsync(id);
            if (existingAddOn == null) return false;

            existingAddOn.Name = updatedAddOn.Name;
            existingAddOn.Description = updatedAddOn.Description;
            existingAddOn.Price = updatedAddOn.Price;
            existingAddOn.Type = updatedAddOn.Type;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var addOn = await _context.AddOns.FindAsync(id);
            if (addOn == null) return false;

            _context.AddOns.Remove(addOn);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
