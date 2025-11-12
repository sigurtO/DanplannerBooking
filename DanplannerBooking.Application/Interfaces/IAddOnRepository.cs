using DanplannerBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface IAddOnRepository
    {
        Task<IEnumerable<AddOn>> GetAllAsync();
        Task<AddOn> GetByIdAsync(Guid id);
        Task CreateAsync(AddOn addOn);
        Task<bool> UpdateAsync(Guid id, AddOn addOn);
        Task<bool> DeleteAsync(Guid id);
    }
}
