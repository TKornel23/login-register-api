using System.ComponentModel.DataAnnotations;

namespace login_register_api.Models
{
    public class LoginRegisterDTO
    {
        [Required]
        [MinLength(3)]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
