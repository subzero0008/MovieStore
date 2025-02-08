using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStoreMvc.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager; // Добавено поле за SignInManager

        // Конструктор, който инжектира UserManager, RoleManager и SignInManager
        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager; // Инжектиране на SignInManager
        }

        // Метод за показване на списъка с потребители
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            var users = await _userManager.Users.ToListAsync(); // Зареждаме всички потребители в паметта

            if (currentUserRoles.Contains("Owner"))
            {
                // Собственикът вижда всички потребители, освен тези с роля "Owner"
                users = users.Where(u => !_userManager.IsInRoleAsync(u, "Owner").Result).ToList();
            }
            else if (currentUserRoles.Contains("Admin"))
            {
                // Администраторът вижда само потребители с роля "User" и не вижда себе си
                users = users
                    .Where(u => _userManager.IsInRoleAsync(u, "User").Result && u.Id != currentUser.Id)
                    .ToList();
            }
            else
            {
                // Никой друг не трябва да има достъп
                return Forbid();
            }

            return View(users);
        }
        // GET метод за показване на формата за редактиране на информацията на потребителя
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Създаване на ViewModel за редактиране на информацията
            var model = new ProfileViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Name = currentUser.Name
            };

            return View(model);
        }

        // POST метод за обработка на промени в информацията на потребителя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Получаване на текущия потребител
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "UserAuthentication");
                }

                // Валидация за текущата парола
                if (!string.IsNullOrEmpty(model.CurrentPassword))
                {
                    var passwordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                    if (!passwordValid)
                    {
                        ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                        return View(model);
                    }
                }

                // Промяна на името и email-а
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Name = model.Name;

                // Проверка дали новата парола съвпада с текущата
                if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.CurrentPassword)
                {
                    ModelState.AddModelError("NewPassword", "The new password cannot be the same as the current password.");
                    return View(model);
                }

                // Промяна на паролата, ако е необходима
                if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.ConfirmNewPassword)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);  // Връщаме обратно с грешки
                    }
                }

                // Обновяване на потребителя
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return RedirectToAction("EditProfile");  // Пренасочване към същата страница
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Някои от данните не са валидни. Моля, проверете и опитайте отново.");
            }

            return View(model); // Връщаме формата с грешки
        }











        // Метод за редактиране на информацията на друг потребител (само за администратор или собственик)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync(); // Зареждаме всички роли
            if (roles == null)
            {
                // В случай че ролите са null, добавяме празен списък
                roles = new List<IdentityRole>();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // Ако userRoles е null, добавяме празен списък
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles ?? new List<string>();

            return View(user);
        }


        // POST метод за редактиране на информацията на друг потребител
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user, string newRole, string newPassword, string confirmNewPassword)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var currentUserRoles = await _userManager.GetRolesAsync(existingUser);

            // Проверка дали администратор се опитва да присвои роля "Owner"
            if (User.IsInRole("Admin") && newRole == "Owner")
            {
                ModelState.AddModelError(string.Empty, "You do not have sufficient privileges to assign the 'Owner' role.");
                return View(user); // Връща потребителя обратно към формата със съобщение за грешка
            }

            // Актуализиране на основната информация
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Name = user.Name;

            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                // Ако има грешки при актуализирането, добавете ги в ModelState
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Създайте нов ProfileViewModel за да върнете обратно в изгледа
                var model = new ProfileViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name
                };

                return View(model); // Използвайте правилния модел тук
            }
;

        

            // Премахваме всички стари роли
            foreach (var role in currentUserRoles)
            {
                var removeRoleResult = await _userManager.RemoveFromRoleAsync(existingUser, role);
                if (!removeRoleResult.Succeeded)
                {
                    foreach (var error in removeRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Добавяме новата роля (проверка дали новата роля не е "Owner" за админ)
            if (!string.IsNullOrEmpty(newRole) && newRole != "Owner" && !currentUserRoles.Contains(newRole))
            {
                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, newRole);

                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Актуализиране на паролата
            if (!string.IsNullOrEmpty(newPassword) && newPassword == confirmNewPassword)
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(existingUser, passwordResetToken, newPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else if (!string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("ConfirmNewPassword", "The new password and confirmation password do not match.");
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }


        // Метод за изтриване на потребител
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserRoles = await _userManager.GetRolesAsync(user);

            // Проверка за права (Админите не могат да трият собственика или други админи)
            if (User.IsInRole("Admin") && currentUserRoles.Contains("Owner"))
            {
                return Forbid();
            }
            if (User.IsInRole("Admin") && currentUserRoles.Contains("Admin"))
            {
                return Forbid();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
