using DanplannerBooking.Application.Dtos.Space;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceController : Controller
    {
        private readonly ISpaceRepository _spaceRepository;

        public SpaceController(ISpaceRepository spaceRepository)
        {
            _spaceRepository = spaceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpaces()
        {
            var spaces = await _spaceRepository.GetAllAsync();
            return Ok(spaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpaceById(Guid id)
        {
            var space = await _spaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return Ok(space);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceDto spaceDto)
        {
            var newSpace = new Space
            {
                CampsiteId = spaceDto.CampsiteId,
                Name = spaceDto.Name,
                Location = spaceDto.Location,
                Description = spaceDto.Description,
                HasElectricity = spaceDto.HasElectricity,
                MetersFromToilet = spaceDto.MetersFromToilet,
                MetersFromPool = spaceDto.MetersFromPool,
                MetersFromPlayground = spaceDto.MetersFromPlayground,
                MetersFromOcean = spaceDto.MetersFromOcean,
                IsAvailable = spaceDto.IsAvailable,
                PricePerNight = spaceDto.PricePerNight,
                Image = spaceDto.Image
            };

            await _spaceRepository.CreateAsync(newSpace);
            return CreatedAtAction(nameof(GetSpaceById), new { id = newSpace.Id }, null);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(Guid id, [FromBody] CreateSpaceDto updatedDto)
        {
            var updatedSpace = new Space
            {
                CampsiteId = updatedDto.CampsiteId,
                Name = updatedDto.Name,
                Location = updatedDto.Location,
                Description = updatedDto.Description,
                HasElectricity = updatedDto.HasElectricity,
                MetersFromToilet = updatedDto.MetersFromToilet,
                MetersFromPool = updatedDto.MetersFromPool,
                MetersFromPlayground = updatedDto.MetersFromPlayground,
                MetersFromOcean = updatedDto.MetersFromOcean,
                IsAvailable = updatedDto.IsAvailable,
                PricePerNight = updatedDto.PricePerNight,
                Image = updatedDto.Image
            };

            var result = await _spaceRepository.UpdateAsync(id, updatedSpace);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(Guid id)
        {
            var result = await _spaceRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}
