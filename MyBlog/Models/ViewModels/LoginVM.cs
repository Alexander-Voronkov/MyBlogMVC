using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
