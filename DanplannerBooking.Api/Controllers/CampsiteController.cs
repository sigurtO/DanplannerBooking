using DanplannerBooking.Application.Dtos.Campsite;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampsiteController : Controller
    {
        private readonly ICampsiteRepository _campsiteRepository;
        public CampsiteController(ICampsiteRepository campsiteRepository)
        {
            _campsiteRepository = campsiteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampsites()
        {
            var campsites = await _campsiteRepository.GetAllAsync();
            return Ok(campsites);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampsiteById(Guid id)
        {
            var campsite = await _campsiteRepository.GetByIdAsync(id);
            if (campsite == null)
            {
                return NotFound();
            }
            return Ok(campsite);
        }


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

        [HttpDelete("{id}")]
        // [Authorize(Policy = "AdminOnly")] //check happens in program.cs
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
