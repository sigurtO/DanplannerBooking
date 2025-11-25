using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Application.Dtos.Campsite;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampsiteController : ControllerBase
{
    private readonly ICampsiteRepository _campsiteRepository;

    public CampsiteController(ICampsiteRepository campsiteRepository)
    
    [ApiController]
    [Route("api/[controller]")]
    public class CampsiteController : Controller
    {
        _campsiteRepository = campsiteRepository;
    }

    // ---------------------------
    // Fuld CRUD til /api/campsite
    // ---------------------------

    // GET: api/campsite
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampsiteResponseDto>>> GetAll()
    {
        var campsites = await _campsiteRepository.GetAllAsync();

        var result = campsites
            .Select(c => new CampsiteResponseDto(
                c.Id,
                c.Name,
                c.Location,
                c.Description,
                c.HasOceanAccess,
                c.HasPool,
                c.HasPlayground,
                c.HasCarCharger
            ))
            .ToList();

        return Ok(result);
    }

    // GET: api/campsite/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CampsiteResponseDto>> GetById(Guid id)
    {
        var campsite = await _campsiteRepository.GetByIdAsync(id);
        if (campsite is null)
            return NotFound();

        var dto = new CampsiteResponseDto(
            campsite.Id,
            campsite.Name,
            campsite.Location,
            campsite.Description,
            campsite.HasOceanAccess,
            campsite.HasPool,
            campsite.HasPlayground,
            campsite.HasCarCharger
        );

        return Ok(dto);
    }

    // POST: api/campsite
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCampsiteDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var entity = new Campsite
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Location = dto.Location,
            Description = dto.Description,
            // ImageUrl kan s�ttes et andet sted � vi lader den st� til default
            HasOceanAccess = dto.HasOceanAccess,
            HasPool = dto.HasPool,
            HasPlayground = dto.HasPlayground,
            HasCarCharger = dto.HasCarCharger
        };

        await _campsiteRepository.CreateAsync(entity);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, null);
    }

    // PUT: api/campsite/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCampsiteDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var updated = new Campsite
        {
            Name = dto.Name,
            Location = dto.Location,
            Description = dto.Description,
            HasOceanAccess = dto.HasOceanAccess,
            HasPool = dto.HasPool,
            HasPlayground = dto.HasPlayground,
            HasCarCharger = dto.HasCarCharger
        };

        var success = await _campsiteRepository.UpdateAsync(id, updated);
        if (!success)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/campsite/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _campsiteRepository.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    // ---------------------------------------------
    // Simpel liste til kort/editor: GET /api/campsites
    // ---------------------------------------------
    [HttpGet("/api/campsites")]
    public async Task<ActionResult<IEnumerable<CampsiteDto>>> GetForMap()
    {
        var campsites = await _campsiteRepository.GetAllAsync();

        var items = campsites
            .Select(c => new CampsiteDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = string.IsNullOrEmpty(c.ImageUrl)
                    ? "images/campsites/default.jpg"
                    : c.ImageUrl
            })
            .OrderBy(c => c.Name)
            .ToList();
            return Ok(items);
        }
    }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateCampsite([FromBody] CreateCampsiteDto campsite)
        {
            var newCampsite = new Campsite // is this still 4 layer structure?
            {
                Name = campsite.Name,
                Location = campsite.Location,
                Description = campsite.Description,
                HasOceanAccess = campsite.HasOceanAccess,
                HasPool = campsite.HasPool,
                HasPlayground = campsite.HasPlayground,
                HasCarCharger = campsite.HasCarCharger
            };
            await _campsiteRepository.CreateAsync(newCampsite);
            return CreatedAtAction(nameof(GetCampsiteById), new { id = newCampsite.Id }, null);

        }
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        // [Authorize(Policy = "AdminOnly")] //check happens in program.cs
        public async Task<IActionResult> UpdateCampsite(Guid id, [FromBody] CreateCampsiteDto updatedCampsite)
        {
            var campsite = new Campsite
            {
                Name = updatedCampsite.Name,
                Location = updatedCampsite.Location,
                Description = updatedCampsite.Description,
                HasOceanAccess = updatedCampsite.HasOceanAccess,
                HasPool = updatedCampsite.HasPool,
                HasPlayground = updatedCampsite.HasPlayground,
                HasCarCharger = updatedCampsite.HasCarCharger
            };
            var result = await _campsiteRepository.UpdateAsync(id, campsite);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampsite(Guid id)
        {
            var result = await _campsiteRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
