using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;
using System.Linq;
using System.Net;

namespace MovieStoreMvc.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        private readonly Cloudinary _cloudinary;

        // Конструкторът приема зависимости чрез DI (Dependency Injection)
        public MovieController(IGenreService genreService, IMovieService movieService, IFileService fileService, Cloudinary cloudinary)
        {
            _movieService = movieService;
            _fileService = fileService;
            _genreService = genreService;
            _cloudinary = cloudinary;
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
        {
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });

            if (!ModelState.IsValid)
                return View(model);

            // Качване на изображение в Cloudinary (ако има)
            if (model.ImageFile != null)
            {
                var uploadResult = UploadImageToCloudinary(model.ImageFile);

                if (uploadResult == null)
                {
                    TempData["msg"] = "File could not be uploaded to Cloudinary";
                    return View(model);
                }

                model.MovieImage = uploadResult.Url.ToString();
            }

            // Добавяне на трейлър (YouTube Video ID)
            if (!string.IsNullOrEmpty(model.YouTubeVideoId))
            {
                // Може да добавиш валидиране тук, ако е нужно
            }

            // Добавяне на филма с YouTube Video ID
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
            if (model.Id > 0)
            {
                ModelState.Remove("ImageFile");
                ModelState.Remove("Genres");
            }

            var selectedGenres = model.Genres;
            var multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList;

            // Проверка за валидността на модела
            if (!ModelState.IsValid)
                return View(model);

            var existingMovie = _movieService.GetById(model.Id);
            if (model.ImageFile == null)
            {
                model.MovieImage = existingMovie.MovieImage;
            }
            else
            {
                var uploadResult = UploadImageToCloudinary(model.ImageFile);
                if (uploadResult == null)
                {
                    TempData["msg"] = "File could not be uploaded to Cloudinary";
                    return View(model);
                }
                model.MovieImage = uploadResult.Url.ToString();
            }

            // Актуализираме трейлъра с новото ID, ако е било въведено
            if (!string.IsNullOrEmpty(model.YouTubeVideoId))
            {
                existingMovie.YouTubeVideoId = model.YouTubeVideoId;
            }
            else
            {
                // Ако не е въведен нов YouTube Video ID, оставяме стария
                model.YouTubeVideoId = existingMovie.YouTubeVideoId;
            }

            // Актуализираме филма заедно с YouTube Video ID и жанровете
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



        // Метод за качване на изображение в Cloudinary
        private UploadResult UploadImageToCloudinary(IFormFile imageFile)
        {
            try
            {
                if (imageFile.Length == 0 || !imageFile.ContentType.StartsWith("image/"))
                {
                    TempData["msg"] = "Invalid file type";
                    return null;
                }

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                };

                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    return uploadResult;  // Връща резултата от качването
                }
                else
                {
                    TempData["msg"] = "Error uploading image to Cloudinary";
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Логване на грешката (ако е необходимо)
                Console.WriteLine(ex.Message);
                TempData["msg"] = "An error occurred while uploading the image";
                return null;
            }
        }

        // Метод за показване на списъка с филми
        public IActionResult MovieList()
        {
            var data = _movieService.List();
            return View(data);
        }

        // Метод за изтриване на филм
        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            return RedirectToAction(nameof(MovieList));
        }
    }
}
