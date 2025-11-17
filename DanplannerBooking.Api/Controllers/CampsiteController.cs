// DanplannerBooking.Api/Controllers/CampsitesController.cs
using DanplannerBooking.Application.Dtos;
using DanplannerBooking.Application.Dtos.Campsite;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanplannerBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampsitesController : ControllerBase
{
    private readonly DbContextBooking _db;
    private readonly ICampsiteRepository _campsiteRepository;

    public CampsitesController(DbContextBooking db, ICampsiteRepository campsiteRepository)
    {
        _db = db;
        _campsiteRepository = campsiteRepository;
    }

    // GET api/campsites
    // Bruges af map-editoren: giver et simpelt DTO med billede
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

    // GET api/campsites/all
    // Fuld liste via repository (fx til admin-UI mv.)
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCampsites()
    {
        var campsites = await _campsiteRepository.GetAllAsync();
        return Ok(campsites);
    }

    // GET api/campsites/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCampsiteById(Guid id)
    {
        var campsite = await _campsiteRepository.GetByIdAsync(id);
