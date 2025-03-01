using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    // Ограничава достъпа до контролера, така че само ауторизирани потребители могат да го използват
    [Authorize]
    public class GenreController : Controller
    {
        //– Стойността може да бъде зададена само веднъж, обикновено в конструктора. Това гарантира,
        //че IGenreService няма да бъде променяно след инициализацията.
        private readonly IGenreService _genreService; // Деклариране на услугата за работа с жанровете

        // Конструктор за инициализация на IGenreService
        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET метод за показване на формата за добавяне на нов жанр
        public IActionResult Add()
        {
            return View();
        }

        // POST метод за обработка на добавянето на нов жанр
        [HttpPost]
        public IActionResult Add(Genre model)
        {
            // Проверява дали моделът е валиден
            if (!ModelState.IsValid)
                return View(model); // Ако не е валиден, връща формата с грешки

            // Извиква метода за добавяне на жанр в базата данни
            var result = _genreService.Add(model);

            // Проверява дали добавянето е успешно
            if (result)
            {
                TempData["msg"] = "Added Successfully"; // Показва съобщение за успех
                return RedirectToAction(nameof(Add)); // Пренасочва към същата страница за добавяне
            }
            else
            {
                TempData["msg"] = "Error on server side"; // Показва съобщение за грешка
                return View(model); // Връща формата с грешка
            }
        }

        // GET метод за показване на формата за редактиране на жанр
        public IActionResult Edit(int id)
        {
            // Вземаме данните за жанра, който ще редактираме
            var data = _genreService.GetById(id);
            return View(data); // Показва формата за редактиране с данните за жанра
        }

        // POST метод за обработка на обновяването на жанра
        [HttpPost]
        public IActionResult Update(Genre model)
        {
            // Проверява дали моделът е валиден
            if (!ModelState.IsValid)
                return View(model); // Ако не е валиден, връща формата с грешки

            // Извиква метода за обновяване на жанра в базата данни
            var result = _genreService.Update(model);

            // Проверява дали обновяването е успешно
            if (result)
            {
                TempData["msg"] = "Updated Successfully"; // Показва съобщение за успех
                return RedirectToAction(nameof(GenreList)); // Пренасочва към списъка с жанрове
            }
            else
            {
                TempData["msg"] = "Error on server side"; // Показва съобщение за грешка
                return View(model); // Връща формата с грешка
            }
        }

        // GET метод за показване на списъка с всички жанрове
        public IActionResult GenreList()
        {
            // Вземаме списък с всички жанрове от услугата
            var data = this._genreService.List().ToList();
            return View(data); // Показва списъка с жанровете
        }

        // Метод за изтриване на жанр
        public IActionResult Delete(int id)
        {
            // Извиква метода за изтриване на жанра от базата данни
            var result = _genreService.Delete(id);
            return RedirectToAction(nameof(GenreList)); // Пренасочва към списъка с жанрове
        }
    }
}
