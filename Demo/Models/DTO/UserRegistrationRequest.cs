using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models.DTO
{
    public class UserRegistrationRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
