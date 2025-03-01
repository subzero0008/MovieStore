using System.ComponentModel.DataAnnotations;

namespace MovieStoreMvc.Models.DTO
{
    public class RegistrationModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")] // Автоматична проверка на формата
        public string Email { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long.")]
        [MaxLength(10, ErrorMessage = "Username cannot be longer than 10 characters.")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain lowercase letters and numbers.")]
        public string Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum length 6 and must contain  1 Uppercase, 1 lowercase, 1 special character and 1 digit")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirm { get; set; }

        public string Role { get; set; }
    }
}
