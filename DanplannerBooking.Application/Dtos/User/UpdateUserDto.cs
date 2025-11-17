using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanplannerBooking.Application.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
    }

    public class AdminUpdateUserDto : UpdateUserDto
    {
        public bool IsAdmin { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class CreateUserDto : UpdateUserDto
    {
        [Required]
        public string Password { get; set; }
    }

    public class RegisterUserDto : UpdateUserDto
    {
        [Required]
        public string Password { get; set; }
    }

    public record UserResponseDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? Country,
    string? Language,
    bool IsAdmin
    );
}
