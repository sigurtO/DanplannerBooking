using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly DbContextBooking _db;
    public UnitsController(DbContextBooking db) => _db = db;

    // GET api/units  -> both Spaces + Cottages
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

        var all = spaces.Concat(cottages).OrderBy(x => x.Name).ToList();
        return Ok(all);
    }

    // PUT api/units/layout  -> bulk update X/Y
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
                if (s is null) continue;
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

    // POST api/units/layout/import -> create/update by name/id and set CampsiteId
    [HttpPost("layout/import")]
    public async Task<ActionResult> ImportLayout([FromBody] ImportMapRequest body, CancellationToken ct)
    {
        try
        {
            if (body?.Units == null || body.Units.Count == 0)
                return BadRequest("No units to import.");

            // Ensure a campsite exists
            var campsite = await _db.Campsites.FirstOrDefaultAsync(ct);
            if (campsite is null)
            {
                campsite = new Campsite
                {
                    Id = Guid.NewGuid(),
                    Name = "Standard Campingplads",
                    Description = "Auto-created by layout import",
                    Location = "N/A"

                };
                await _db.Campsites.AddAsync(campsite, ct);
                await _db.SaveChangesAsync(ct);
            }
            var campsiteId = campsite.Id;

            // Load existing
            var spaces = await _db.Spaces.ToListAsync(ct);
            var cottages = await _db.Cottages.ToListAsync(ct);

            // Lookups
            static string Key(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();
            var spaceById = spaces.ToDictionary(s => s.Id);
            var cottageById = cottages.ToDictionary(c => c.Id);
            var spaceByName = spaces.GroupBy(s => Key(s.Name)).ToDictionary(g => g.Key, g => g.First());
            var cottageByName = cottages.GroupBy(c => Key(c.Name)).ToDictionary(g => g.Key, g => g.First());

            int updated = 0, created = 0, skipped = 0;

            foreach (var u in body.Units)
            {
                var type = (u.Type ?? string.Empty).Trim();
                var isSpace = string.Equals(type, "Space", StringComparison.OrdinalIgnoreCase);
                var isCottage = string.Equals(type, "Cottage", StringComparison.OrdinalIgnoreCase);
                if (!isSpace && !isCottage) { skipped++; continue; }

                if (isSpace)
                {
                    Space? entity = null;

                    if (u.Id.HasValue && spaceById.TryGetValue(u.Id.Value, out var sById))
                        entity = sById;
                    else if (!string.IsNullOrWhiteSpace(u.Name))
                        spaceByName.TryGetValue(Key(u.Name), out entity);

                    if (entity is null)
                    {
                        if (!body.CreateMissing) { skipped++; continue; }
                        entity = new Space
                        {
                            Id = u.Id ?? Guid.NewGuid(),
                            Name = string.IsNullOrWhiteSpace(u.Name) ? "Plads" : u.Name!,
                            CampsiteId = campsiteId,
                            X = u.X,
                            Y = u.Y,

                            // ✅ These fix the DB NOT NULL errors:
                            ImageUrl = "",         // your DB requires non-null
                            PricePerNight = 0m     // (only if non-nullable in model)
                        };
                        _db.Spaces.Add(entity);
                        created++;
                    }
                    else
                    {
                        entity.X = u.X; entity.Y = u.Y;
                        updated++;
                    }
                }
                else // Cottage
                {
                    Cottage? entity = null;

                    if (u.Id.HasValue && cottageById.TryGetValue(u.Id.Value, out var cById))
                        entity = cById;
                    else if (!string.IsNullOrWhiteSpace(u.Name))
                        cottageByName.TryGetValue(Key(u.Name), out entity);

                    if (entity is null)
                    {
                        if (!body.CreateMissing) { skipped++; continue; }
                        entity = new Cottage
                        {
                            Id = u.Id ?? Guid.NewGuid(),
                            Name = string.IsNullOrWhiteSpace(u.Name) ? "Hytte" : u.Name!,
                            CampsiteId = campsiteId,
                            X = u.X,
                            Y = u.Y,

                            // ✅ satisfy NOT NULL columns in your DB schema:
                            Description = "N/A",
                            ImageUrl = "",     // <-- add this line
                                               // If non-nullable in your model:
                                               // PricePerNight = 0m
                        };
                        _db.Cottages.Add(entity);
                        created++;
                    }
                    else
                    {
                        entity.X = u.X; entity.Y = u.Y;
                        updated++;
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { updated, created, skipped });
        }
        catch (DbUpdateException ex)
        {
            Console.Error.WriteLine(ex);
            return Problem(ex.InnerException?.Message ?? ex.Message, statusCode: 500);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return Problem(ex.ToString(), statusCode: 500);
        }
    }
}
