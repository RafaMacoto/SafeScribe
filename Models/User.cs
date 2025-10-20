using System.ComponentModel.DataAnnotations;

namespace SafeScribe.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = null!;

        
        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string Role { get; set; } = SafeScribe.Models.Role.Reader;
    }
}
