using DanplannerBooking.Application.Dtos.Cottage;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> UpdateCottage(Guid id, [FromBody] CottageUpdateDto cottageDto)
        {
            var cottageToUpdate = new Cottage
            {
                Id = id,
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
                Image = cottageDto.Image,
                CampsiteId = cottageDto.CampsiteId,
                RowVersion = cottageDto.RowVersion // client must send this back
            };

            try
            {
                var success = await _cottageRepository.UpdateAsync(id, cottageToUpdate);
                if (!success)
                    return NotFound();

                return NoContent(); // update succeeded
            }
            catch (DbUpdateConcurrencyException)
            {
                // return HTTP 409 Conflict if rowversion mismatch
                return Conflict("The cottage was modified by another user. Please reload and try again.");
            }
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
