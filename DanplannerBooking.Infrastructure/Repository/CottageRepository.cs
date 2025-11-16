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
    public class CottageRepository : ICottageRepository
    {
        private readonly DbContextBooking _context;

        public CottageRepository(DbContextBooking context)
        {
            _context = context;
        }

        public async Task CreateAsync(Cottage cottage)
        {
            _context.Cottages.Add(cottage);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cottage>> GetAllAsync()
        {
            return await _context.Cottages.ToListAsync();
        }

        public async Task<Cottage> GetByIdAsync(Guid id)
        {
            return await _context.Cottages.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Guid id, Cottage cottageUpdated)
        {
            var existingCottage = await _context.Cottages.FindAsync(id);
            if (existingCottage == null) return false;

            existingCottage.Name = cottageUpdated.Name;
            existingCottage.Location = cottageUpdated.Location;
            existingCottage.Description = cottageUpdated.Description;
            existingCottage.HasToilet = cottageUpdated.HasToilet;
            existingCottage.HasShower = cottageUpdated.HasShower;
            existingCottage.HasKitchen = cottageUpdated.HasKitchen;
            existingCottage.HasHeating = cottageUpdated.HasHeating;
            existingCottage.HasWiFi = cottageUpdated.HasWiFi;
            existingCottage.IsAvailable = cottageUpdated.IsAvailable;
            existingCottage.PricePerNight = cottageUpdated.PricePerNight;
            existingCottage.Image = cottageUpdated.Image;
            existingCottage.CampsiteId = cottageUpdated.CampsiteId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cottage = await _context.Cottages.FindAsync(id);
            if (cottage == null) return false;

            _context.Cottages.Remove(cottage);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
