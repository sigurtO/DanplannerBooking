using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DanplannerBooking.Domain.Entities;
namespace DanplannerBooking.Application.Interfaces
{
    public interface ISpaceRepository
    {
        Task<IEnumerable<Space>> GetAllAsync();
        Task<Space> GetByIdAsync(Guid id);
        Task CreateAsync(Space space);
        Task<bool> UpdateAsync(Guid id, Space space);
        Task<bool> DeleteAsync(Guid id);
    }
}
