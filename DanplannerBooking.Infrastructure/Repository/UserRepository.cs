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
    public class UserRepository : IUserRepository
    {
        private readonly DbContextBooking _context; // Dependency Injection

        public UserRepository(DbContextBooking context) // Constructor Injection
        {
            _context = context;
        }
        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
 
        public async Task<bool> UpdateAsync(Guid id, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Country = updatedUser.Country;
            existingUser.Language = updatedUser.Language;
            existingUser.IsAdmin = updatedUser.IsAdmin;
            //we dont update password

            await _context.SaveChangesAsync();
            return true;
        }
    }
}