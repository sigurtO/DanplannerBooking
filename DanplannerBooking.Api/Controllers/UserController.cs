using DanplannerBooking.Application.Dtos.User;
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        //Get api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers() //Ienum a list of users
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAdmins()
        {
            var users = await _userRepository.GetAllAsync();
            var admins = users.Where(u => u.Role == "Admin").ToList();

            return Ok(admins);
        }


        //Post api/user
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Phone = createUserDto.Phone,
                Country = createUserDto.Country,
                Language = createUserDto.Language,

                // Brug den role klienten sender – fallback til "User"
                Role = string.IsNullOrWhiteSpace(createUserDto.Role)
            ? "User"
            : createUserDto.Role,

                Password = createUserDto.Password // TODO: Hash password senere
            };
            await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, null);
        }


        //Put api/user/me
        [HttpPut("me")] //Update user by id (non admin) //not used for edit profile
        [Authorize]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get user id from token
            if (userId == null) return Unauthorized();

            var user = new User
            {
                Name = updateUserDto.Name,
                Email = updateUserDto.Email,
                Phone = updateUserDto.Phone,
                Country = updateUserDto.Country,
                Language = updateUserDto.Language
            };
            var result = await _userRepository.UpdateAsync(Guid.Parse(userId), user);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}")] //Update user by id (admin)
                          // [Authorize(Policy = "AdminOnly")] //check happens in program.cs
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateUserDto adminUpdateUserDto)
        {
            var user = new User
            {
                Name = adminUpdateUserDto.Name,
                Email = adminUpdateUserDto.Email,
                Phone = adminUpdateUserDto.Phone,
                Country = adminUpdateUserDto.Country,
                Language = adminUpdateUserDto.Language,
                Role = adminUpdateUserDto.Role
            };
            var result = await _userRepository.UpdateAsync(id, user);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        //Delete api/user/{id}
        [HttpDelete("{id}")]
        // [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
