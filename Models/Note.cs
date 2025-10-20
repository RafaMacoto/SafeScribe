using System.ComponentModel.DataAnnotations;

namespace SafeScribe.Models
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relaciona com o dono (User.Id)
        [Required]
        public Guid UserId { get; set; }
    }
}
