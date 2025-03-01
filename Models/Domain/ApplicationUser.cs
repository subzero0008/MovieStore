using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStoreMvc.Models.Domain
{
    // Класът ApplicationUser разширява IdentityUser и добавя допълнителни полета за потребителя
    public class ApplicationUser : IdentityUser
    {
        // Име на потребителя (допълнително поле)
        public string Name { get; set; }

        // Полетата по-долу няма да се съхраняват в базата данни (маркирани с [NotMapped])

        [NotMapped] // Поле за новата парола (използва се при промяна на парола)
        public string NewPassword { get; set; }

        [NotMapped] // Поле за потвърждение на новата парола
        public string ConfirmPassword { get; set; }
    }
}
// [NotMapped] означава, че полето не се включва в базовата структура на базата данни, а се използва само в приложението. 






