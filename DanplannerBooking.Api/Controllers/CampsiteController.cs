// DanplannerBooking.Api/Controllers/CampsitesController.cs
using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampsitesController : ControllerBase
{
    private readonly DbContextBooking _db;
    public CampsitesController(DbContextBooking db) => _db = db;

    // GET api/campsites
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampsiteDto>>> Get(CancellationToken ct)
    {
        var items = await _db.Campsites.AsNoTracking()
            .Select(c => new CampsiteDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = string.IsNullOrEmpty(c.ImageUrl)
                    ? "images/campsites/default.jpg"
                    : c.ImageUrl
            })
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return Ok(items);
    }
}
