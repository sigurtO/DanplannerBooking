using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly DbContextBooking _db;
    public UnitsController(DbContextBooking db) => _db = db;

    // GET api/units  -> return both Spaces + Cottages as a single list
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits(CancellationToken ct)
    {
        var spaces = await _db.Spaces.AsNoTracking()
            .Select(s => new UnitDto
            {
                Id = s.Id,
                Name = s.Name,
                Type = "Space",
                X = s.X,
                Y = s.Y
            }).ToListAsync(ct);

        var cottages = await _db.Cottages.AsNoTracking()
            .Select(c => new UnitDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = "Cottage",
                X = c.X,
                Y = c.Y
            }).ToListAsync(ct);

        // Union and (optionally) order by Name
        var all = spaces.Concat(cottages).OrderBy(x => x.Name).ToList();
        return Ok(all);
    }

    // PUT api/units/layout  -> bulk update X/Y for both types
    [HttpPut("layout")]
    public async Task<ActionResult> SaveLayout([FromBody] List<UnitLayoutDto> layout, CancellationToken ct)
    {
        if (layout is null || layout.Count == 0) return BadRequest("Empty layout");

        var spaceIds = layout.Where(l => l.Type == "Space").Select(l => l.Id).ToHashSet();
        var cottageIds = layout.Where(l => l.Type == "Cottage").Select(l => l.Id).ToHashSet();

        var dbSpaces = await _db.Spaces.Where(s => spaceIds.Contains(s.Id)).ToListAsync(ct);
        var dbCottages = await _db.Cottages.Where(c => cottageIds.Contains(c.Id)).ToListAsync(ct);

        foreach (var item in layout)
        {
            if (item.Type == "Space")
            {
                var s = dbSpaces.FirstOrDefault(x => x.Id == item.Id);
                if (s is null) continue; // or handle NotFound
                s.X = item.X; s.Y = item.Y;
            }
            else if (item.Type == "Cottage")
            {
                var c = dbCottages.FirstOrDefault(x => x.Id == item.Id);
                if (c is null) continue;
                c.X = item.X; c.Y = item.Y;
            }
        }

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
   
    [HttpPost("layout/import")]
    public async Task<ActionResult> ImportLayout([FromBody] ImportMapRequest body, CancellationToken ct)
    {
        if (body?.Units == null || body.Units.Count == 0)
            return BadRequest("No units to import.");

        // Load existing sets once
        var spaces = await _db.Spaces.ToListAsync(ct);
        var cottages = await _db.Cottages.ToListAsync(ct);

        // Fast lookups
        var spaceById = spaces.ToDictionary(s => s.Id, s => s);
        var cottageById = cottages.ToDictionary(c => c.Id, c => c);

        // Name lookups (case-insensitive)
        var spaceByName = spaces.GroupBy(s => s.Name.Trim().ToLowerInvariant())
                                .ToDictionary(g => g.Key, g => g.First());
        var cottageByName = cottages.GroupBy(c => c.Name.Trim().ToLowerInvariant())
                                    .ToDictionary(g => g.Key, g => g.First());

        int updated = 0, created = 0, skipped = 0;

        foreach (var u in body.Units)
        {
            var type = (u.Type ?? "").Trim();
            var isSpace = string.Equals(type, "Space", StringComparison.OrdinalIgnoreCase);
            var isCottage = string.Equals(type, "Cottage", StringComparison.OrdinalIgnoreCase);

            if (!isSpace && !isCottage) { skipped++; continue; }

            if (isSpace)
            {
                Space? entity = null;

                if (u.Id.HasValue && spaceById.TryGetValue(u.Id.Value, out var sById))
                    entity = sById;
                else if (!string.IsNullOrWhiteSpace(u.Name))
                {
                    var key = u.Name.Trim().ToLowerInvariant();
                    spaceByName.TryGetValue(key, out entity);
                }

                if (entity is null)
                {
                    if (!body.CreateMissing) { skipped++; continue; }
                    entity = new Space
                    {
                        Id = u.Id ?? Guid.NewGuid(),
                        Name = string.IsNullOrWhiteSpace(u.Name) ? "Plads" : u.Name,
                        CampsiteId = Guid.Empty, // set a real campsite id if you want to attach them
                        X = u.X,
                        Y = u.Y
                    };
                    _db.Spaces.Add(entity);
                    created++;
                }
                else
                {
                    entity.X = u.X; entity.Y = u.Y; updated++;
                }
            }
            else // Cottage
            {
                Cottage? entity = null;

                if (u.Id.HasValue && cottageById.TryGetValue(u.Id.Value, out var cById))
                    entity = cById;
                else if (!string.IsNullOrWhiteSpace(u.Name))
                {
                    var key = u.Name.Trim().ToLowerInvariant();
                    cottageByName.TryGetValue(key, out entity);
                }

                if (entity is null)
                {
                    if (!body.CreateMissing) { skipped++; continue; }
                    entity = new Cottage
                    {
                        Id = u.Id ?? Guid.NewGuid(),
                        Name = string.IsNullOrWhiteSpace(u.Name) ? "Hytte" : u.Name,
                        CampsiteId = Guid.Empty, // set as appropriate
                        X = u.X,
                        Y = u.Y
                    };
                    _db.Cottages.Add(entity);
                    created++;
                }
                else
                {
                    entity.X = u.X; entity.Y = u.Y; updated++;
                }
            }
        }

        await _db.SaveChangesAsync(ct);

        return Ok(new { updated, created, skipped });
    }
}
