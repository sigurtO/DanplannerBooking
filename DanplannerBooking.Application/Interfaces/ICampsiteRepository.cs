using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface ICampsiteRepository
    {
        Task<IEnumerable<Campsite>> GetAllAsync();
        Task<Campsite> GetByIdAsync(Guid id);
        Task CreateAsync(Campsite campsite);
        Task<bool> UpdateAsync(Guid id, Campsite campsite);
        Task<bool> DeleteAsync(Guid id);
    }
}
