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
    public class SpaceRepository : ISpaceRepository
    {
        private readonly DbContextBooking _context;
        public SpaceRepository(DbContextBooking context)
        {
            _context = context;
        }

        public async Task CreateAsync(Space space)
        {
            _context.Spaces.Add(space);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var space = await _context.Spaces.FindAsync(id);
            if (space == null) return false;

            _context.Spaces.Remove(space);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Space>> GetAllAsync()
        {
            return await _context.Spaces.ToListAsync();
        }

        public async Task<Space> GetByIdAsync(Guid id)
        {
            return await _context.Spaces.FindAsync(id);

        }

        public async Task<bool> UpdateAsync(Guid id, Space updatedSpace)
        {
            var existingSpace = await _context.Spaces.FindAsync(id);
            if (existingSpace == null) return false;
    
            existingSpace.Name = updatedSpace.Name;
            existingSpace.Location = updatedSpace.Location;
            existingSpace.Description = updatedSpace.Description;
            existingSpace.CampsiteId = updatedSpace.CampsiteId;

            existingSpace.HasElectricity = updatedSpace.HasElectricity;
            existingSpace.MetersFromToilet = updatedSpace.MetersFromToilet;
            existingSpace.MetersFromPool = updatedSpace.MetersFromPool;
            existingSpace.MetersFromPlayground = updatedSpace.MetersFromPlayground;
            existingSpace.MetersFromOcean = updatedSpace.MetersFromOcean;
            existingSpace.IsAvailable = updatedSpace.IsAvailable;
            existingSpace.PricePerNight = updatedSpace.PricePerNight;

            if (updatedSpace.Image != null && updatedSpace.Image.Length > 0)
            {
                existingSpace.Image = updatedSpace.Image;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}