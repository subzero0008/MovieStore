using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStoreMvc.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

            [NotMapped]
            public string NewPassword { get; set; }

            [NotMapped]
            public string ConfirmPassword { get; set; }
        }
    }