namespace DanplannerBooking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }  // Recommended primary key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public bool IsAdmin { get; set; }
    }





}
