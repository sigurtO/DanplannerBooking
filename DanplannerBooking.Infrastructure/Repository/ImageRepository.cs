using DanplannerBooking.Application.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Infrastructure.Repository
{
    public class ImageRepository : IImageRepository
    {
        public bool ValidateFile(IBrowserFile file, int maxSize) =>
                file.Size <= maxSize && file.ContentType.StartsWith("image/");

        public async Task<byte[]> ConvertToBytesAsync(IBrowserFile file, int maxSize)
        {
            using var stream = new MemoryStream();
            await file.OpenReadStream(maxSize).CopyToAsync(stream);
            return stream.ToArray();
        }
    }
}
