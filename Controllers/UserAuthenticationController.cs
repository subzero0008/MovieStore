using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;
using System.Threading.Tasks;

namespace MovieStoreMvc.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService authService;

        public UserAuthenticationController(IUserAuthenticationService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Ако не е избрана роля, задаваме по подразбиране "User"
            if (string.IsNullOrEmpty(model.Role))
            {
                model.Role = "User"; // Може да зададеш друга роля по подразбиране, ако искаш.
            }

            var result = await authService.RegisterAsync(model);

            // Проверка на резултата и добавяне на съобщения в TempData
            if (result.StatusCode == 1)
            {
                TempData["SuccessMessage"] = result.Message;  // Съобщение за успешна регистрация
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;  // Съобщение за грешка
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginAsync(model);
            if (result.StatusCode == 1)
                return RedirectToAction("Index", "Home");
            else
            {
                TempData["ErrorMessage"] = "Could not log in..";  // Съобщение за неуспешен вход
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
