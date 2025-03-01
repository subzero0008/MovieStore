using System.ComponentModel.DataAnnotations;

namespace MovieStoreMvc.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Name = string.Empty;
            CurrentPassword = string.Empty;
        }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "New Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
