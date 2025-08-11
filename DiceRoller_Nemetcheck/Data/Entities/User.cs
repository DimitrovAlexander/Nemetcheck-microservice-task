using System.ComponentModel.DataAnnotations;

namespace UserMicroservice.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}
