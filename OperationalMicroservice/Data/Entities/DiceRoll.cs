using System.ComponentModel.DataAnnotations;

namespace OperationalMicroservice.Data.Entities
{
    public class DiceRoll
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } // FK reference to UserService user id (Guid in token)

        [Required]
        public int Die1 { get; set; }

        [Required]
        public int Die2 { get; set; }

        [Required]
        public int Sum => Die1 + Die2;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
