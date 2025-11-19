namespace DanplannerBooking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }  // Recommended primary key
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string Role { get; set; } = "User"; // Admin / User
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }





}
