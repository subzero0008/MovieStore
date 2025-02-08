using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;

namespace MovieStoreMvc.Controllers
{
    [Authorize(Roles = "Owner")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Показва списъка с всички потребители
        public async Task<IActionResult> ManageUsers()
        {
            // Вземаме всички потребители, без да се филтрираме по роли
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }




        // Задаване на роля на потребител
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Ролята не съществува.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Проверяваме дали потребителят вече има тази роля
            if (!currentRoles.Contains(role))
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    // Ролята е добавена успешно, сега презареждаме списъка с потребители
                    var users = await _userManager.Users.ToListAsync(); // Обновяваме потребителите отново
                    return RedirectToAction("ManageUsers", users); // Пренасочваме към обновения списък
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return RedirectToAction("ManageUsers"); // Ако роля вече е присъединена, няма нужда да обновяваме списъка
        }
    }
}