using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Application.Dtos.AddOn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddOnController : Controller
    {
        private readonly IAddOnRepository _addOnRepository;

        public AddOnController(IAddOnRepository addOnRepository)
        {
            _addOnRepository = addOnRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddOns()
        {
            var addOns = await _addOnRepository.GetAllAsync();
            return Ok(addOns);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddOnById(Guid id)
        {
            var addOn = await _addOnRepository.GetByIdAsync(id);
            if (addOn == null)
            {
                return NotFound();
            }
            return Ok(addOn);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAddOn([FromBody] AddOnDto addOnDto)
        {
            var newAddOn = new AddOn
            {
                Name = addOnDto.Name,
                Description = addOnDto.Description,
                Price = addOnDto.Price,
                Type = addOnDto.Type
            };

            await _addOnRepository.CreateAsync(newAddOn);
            return CreatedAtAction(nameof(GetAddOnById), new { id = newAddOn.Id }, null);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddOn(Guid id, [FromBody] AddOnDto updatedDto)
        {
            var updatedAddOn = new AddOn
            {
                Name = updatedDto.Name,
                Description = updatedDto.Description,
                Price = updatedDto.Price,
                Type = updatedDto.Type
            };

            var result = await _addOnRepository.UpdateAsync(id, updatedAddOn);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddOn(Guid id)
        {
            var result = await _addOnRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
