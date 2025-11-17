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
    public class CampsiteRepository : ICampsiteRepository
    {
        private readonly DbContextBooking _context; // Dependency Injection
        
        public CampsiteRepository(DbContextBooking context)
        {
            _context = context;
        }

        public async Task CreateAsync(Campsite campsite)
        {
            _context.Campsites.Add(campsite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
           var campsite = await _context.Campsites.FindAsync(id);
              if (campsite == null) return false;

              _context.Campsites.Remove(campsite);
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<IEnumerable<Campsite>> GetAllAsync()
        {
            return await _context.Campsites.ToListAsync();
        }

        public async Task<Campsite> GetByIdAsync(Guid id)
        {
            return await _context.Campsites.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Guid id, Campsite campsiteUpdated)
        {
            var existingCampsite = await _context.Campsites.FindAsync(id);

            if (existingCampsite == null) return false;

            existingCampsite.Name = campsiteUpdated.Name;
            existingCampsite.Location = campsiteUpdated.Location;
            existingCampsite.Description = campsiteUpdated.Description;
            existingCampsite.HasOceanAccess = campsiteUpdated.HasOceanAccess;
            existingCampsite.HasPool = campsiteUpdated.HasPool;
            existingCampsite.HasPlayground = campsiteUpdated.HasPlayground;
            existingCampsite.HasCarCharger = campsiteUpdated.HasCarCharger;

            await _context.SaveChangesAsync();
            return true;

        }
    }
}
