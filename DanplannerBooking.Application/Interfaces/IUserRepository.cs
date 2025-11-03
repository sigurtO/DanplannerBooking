using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
        Task<bool> UpdateAsync(Guid id, User user);
        Task<bool> DeleteAsync(Guid id);
    }
}
