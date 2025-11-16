using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using DanplannerBooking.Application.Dtos.Cottage;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CottageController : Controller
    {
        private readonly ICottageRepository _cottageRepository;

        public CottageController(ICottageRepository cottageRepository)
        {
            _cottageRepository = cottageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCottages()
        {
            var cottages = await _cottageRepository.GetAllAsync();
            return Ok(cottages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCottageById(Guid id)
        {
            var cottage = await _cottageRepository.GetByIdAsync(id);
            if (cottage == null)
            {
                return NotFound();
            }
            return Ok(cottage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCottage([FromBody] CottageDto cottageDto)
        {
            var newCottage = new Cottage
            {
                CampsiteId = cottageDto.CampsiteId,
                Name = cottageDto.Name,
                Location = cottageDto.Location,
                Description = cottageDto.Description,
                HasToilet = cottageDto.HasToilet,
                HasShower = cottageDto.HasShower,
                HasKitchen = cottageDto.HasKitchen,
                HasHeating = cottageDto.HasHeating,
                HasWiFi = cottageDto.HasWiFi,
                IsAvailable = cottageDto.IsAvailable,
                PricePerNight = cottageDto.PricePerNight,
                Image = cottageDto.Image
            };

            await _cottageRepository.CreateAsync(newCottage);
            return CreatedAtAction(nameof(GetCottageById), new { id = newCottage.Id }, null);
        }

        [HttpPut("{id}")]
        // [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCottage(Guid id, [FromBody] CottageDto updatedDto)
        {
            var updatedCottage = new Cottage
            {
                CampsiteId = updatedDto.CampsiteId,
                Name = updatedDto.Name,
                Location = updatedDto.Location,
                Description = updatedDto.Description,
                HasToilet = updatedDto.HasToilet,
                HasShower = updatedDto.HasShower,
                HasKitchen = updatedDto.HasKitchen,
                HasHeating = updatedDto.HasHeating,
                HasWiFi = updatedDto.HasWiFi,
                IsAvailable = updatedDto.IsAvailable,
                PricePerNight = updatedDto.PricePerNight,
                Image = updatedDto.Image
            };

            var result = await _cottageRepository.UpdateAsync(id, updatedCottage);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }


        [HttpDelete("{id}")]
        // [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCottage(Guid id)
        {
            var result = await _cottageRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}
