using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Interfaces
{
    public interface IImageRepository
    {
        Task<byte[]> ConvertToBytesAsync(IBrowserFile file, int maxSize);
        bool ValidateFile(IBrowserFile file, int maxSize);
    }
}
