using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Application.Dtos.Campsite;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampsiteController : ControllerBase
{
    private readonly ICampsiteRepository _campsiteRepository;

    public CampsiteController(ICampsiteRepository campsiteRepository)
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
                c.HasCarCharger,
                c.Image
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
            campsite.HasCarCharger,
            campsite.Image
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
            Image = dto.Image,
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

   
}

