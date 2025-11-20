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

    public UnitsController(DbContextBooking db)
    {
        _db = db;
    }

    // Små interne DTOs til options-endpointet
    public sealed class UnitOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";     // "Space" | "Cottage"
        public bool HasPosition { get; set; }      // true hvis (X,Y) != (0,0)
        public Guid CampsiteId { get; set; }
    }

    // ----------------------------
    // GET api/units?campsiteId=...
    // Bruges af map + editor til at hente alle enheder for en plads
    // ----------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits(
        [FromQuery] Guid? campsiteId,
        CancellationToken ct)
    {
        if (campsiteId is null || campsiteId == Guid.Empty)
        {
            return Ok(Array.Empty<UnitDto>());
        }

        var spacesQ = _db.Spaces
            .Where(s => s.CampsiteId == campsiteId)
            .Select(s => new UnitDto
            {
                Id = s.Id,
                Name = s.Name,
                Type = "Space",
                X = s.X,
                Y = s.Y,
                CampsiteId = s.CampsiteId
            })
            .AsNoTracking();

        var cottagesQ = _db.Cottages
            .Where(c => c.CampsiteId == campsiteId)
            .Select(c => new UnitDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = "Cottage",
                X = c.X,
                Y = c.Y,
                CampsiteId = c.CampsiteId
            })
            .AsNoTracking();

        var result = await spacesQ
            .Concat(cottagesQ)
            .OrderBy(u => u.Type)
            .ThenBy(u => u.Name)
            .ToListAsync(ct);

        return Ok(result);
    }

    // -----------------------------------------------
    // GET api/units/options?campsiteId=...&unplacedOnly=true
    // Bruges af editoren til dropdown "Placer DB-enhed"
    // -----------------------------------------------
    [HttpGet("options")]
    public async Task<ActionResult<IEnumerable<UnitOptionDto>>> GetUnitOptions(
        [FromQuery] Guid campsiteId,
        [FromQuery] bool unplacedOnly = false,
        CancellationToken ct = default)
    {
        if (campsiteId == Guid.Empty)
            return Ok(Array.Empty<UnitOptionDto>());

        var spacesQ = _db.Spaces
            .Where(s => s.CampsiteId == campsiteId)
            .Select(s => new UnitOptionDto
            {
                Id = s.Id,
                Name = s.Name,
                Type = "Space",
                HasPosition = s.X != 0 || s.Y != 0,
                CampsiteId = s.CampsiteId
            });

        var cottagesQ = _db.Cottages
            .Where(c => c.CampsiteId == campsiteId)
            .Select(c => new UnitOptionDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = "Cottage",
                HasPosition = c.X != 0 || c.Y != 0,
                CampsiteId = c.CampsiteId
            });

        var all = await spacesQ
            .Concat(cottagesQ)
            .OrderBy(u => u.Type)
            .ThenBy(u => u.Name)
            .ToListAsync(ct);

        if (unplacedOnly)
            all = all.Where(u => !u.HasPosition).ToList();

        return Ok(all);
    }

    // ----------------------------
    // PUT api/units/{id}
    // Bruges hvis du vil opdatere en enkelt enhed (navn, X/Y, osv.)
    // ----------------------------
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UnitDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Vi finder enten en Space eller Cottage med det id
        var space = await _db.Spaces.FirstOrDefaultAsync(s => s.Id == id);
        var cottage = space is null
            ? await _db.Cottages.FirstOrDefaultAsync(c => c.Id == id)
            : null;

        if (space is null && cottage is null)
            return NotFound();

        if (space is not null)
        {
            space.Name = dto.Name;
            space.X = dto.X;
            space.Y = dto.Y;
        }
        else if (cottage is not null)
        {
            cottage.Name = dto.Name;
            cottage.X = dto.X;
            cottage.Y = dto.Y;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ---------------------------------------------------
    // PUT api/units/layout?campsiteId=...
    // Bruges af editoren til at gemme ALLE X/Y-positioner
    // ---------------------------------------------------
    [HttpPut("layout")]
    public async Task<IActionResult> SaveLayout(
        [FromQuery] Guid campsiteId,
        [FromBody] List<UnitLayoutDto> layout,
        CancellationToken ct)
    {
        if (campsiteId == Guid.Empty)
            return BadRequest("campsiteId is required.");

        if (layout is null || layout.Count == 0)
            return BadRequest("Layout is empty.");

        // Hent alle spaces og cottages for denne campsite
        var spaces = await _db.Spaces
            .Where(s => s.CampsiteId == campsiteId)
            .ToListAsync(ct);

        var cottages = await _db.Cottages
            .Where(c => c.CampsiteId == campsiteId)
            .ToListAsync(ct);

        // Opdater X/Y baseret på layout-listen
        foreach (var item in layout)
        {
            if (item.Type.Equals("Space", StringComparison.OrdinalIgnoreCase))
            {
                var s = spaces.FirstOrDefault(x => x.Id == item.Id);
                if (s != null)
                {
                    s.X = item.X;
                    s.Y = item.Y;
                }
            }
            else if (item.Type.Equals("Cottage", StringComparison.OrdinalIgnoreCase))
            {
                var c = cottages.FirstOrDefault(x => x.Id == item.Id);
                if (c != null)
                {
                    c.X = item.X;
                    c.Y = item.Y;
                }
            }
        }

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
