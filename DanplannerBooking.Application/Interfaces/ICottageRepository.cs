using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface ICottageRepository
    {
        Task<IEnumerable<Cottage>> GetAllAsync();
        Task<Cottage> GetByIdAsync(Guid id);
        Task CreateAsync(Cottage cottage);
        Task<bool> UpdateAsync(Guid id, Cottage cottage);
        Task<bool> DeleteAsync(Guid id);
    }
}
