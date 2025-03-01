using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;
using System.Linq;

namespace MovieStoreMvc.Controllers
{
    [Authorize]

    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        // Конструкторът приема зависимости чрез DI (Dependency Injection)

        public MovieController(IGenreService genreService, IMovieService movieService, IFileService fileService)
        {
            _movieService = movieService;
            _fileService = fileService;
            _genreService = genreService;
        }
        // Метод за показване на формата за добавяне на нов филм

        public IActionResult Add()
        {
            var model = new Movie
            {
                GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() })
            };
            return View(model);
        }
        // Метод за обработка на POST заявката при добавяне на нов филм

        [HttpPost]
        public IActionResult Add(Movie model)
        {            // Зарежда жанровете отново, за да се покажат правилно при грешка във валидацията

            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            if (!ModelState.IsValid)
                return View(model);

            if (model.ImageFile != null)
            {
                var fileResult = _fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File could not be saved";
                    return View(model);
                }
                model.MovieImage = fileResult.Item2;
            }
            // Запазва филма в базата данни

            var result = _movieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }
        // Метод за зареждане на формата за редактиране на съществуващ филм

        public IActionResult Edit(int id)
        {
            var model = _movieService.GetById(id);
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);

            // Зареждаме списъка с жанровете
            var genreList = _genreService.List().Select(a => new SelectListItem
            {
                Text = a.GenreName,
                Value = a.Id.ToString()
            }).ToList();

            // Създаваме MultiSelectList, която да съдържа избраните жанрове
            model.MultiGenreList = new MultiSelectList(genreList, "Value", "Text", selectedGenres);

            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(Movie model)
        {
            // Ако редактираме филм, премахваме задължителните полета за ImageFile и Genres
            if (model.Id > 0) // Ако е редакция
            {
                ModelState.Remove("ImageFile");  // Премахваме задължителност за изображението
                ModelState.Remove("Genres");    // Премахваме задължителност за жанровете
            }

            var selectedGenres = model.Genres;
            var multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList;

            if (!ModelState.IsValid)
                return View(model);

            // Ако не е качено ново изображение, запазваме старото
            var existingMovie = _movieService.GetById(model.Id);
            if (model.ImageFile == null)
            {
                model.MovieImage = existingMovie.MovieImage;
            }
            else
            {
                var fileResult = _fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File could not be saved";
                    return View(model);
                }
                model.MovieImage = fileResult.Item2;
            }

            // Актуализираме филма
            var result = _movieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Updated Successfully";
                return RedirectToAction(nameof(MovieList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }



        public IActionResult MovieList()
        {
            var data = _movieService.List();
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            return RedirectToAction(nameof(MovieList));
        }
    }
}
