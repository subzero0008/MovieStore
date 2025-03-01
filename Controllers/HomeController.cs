using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;

        public HomeController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index(string term = "", int currentPage = 1)
        {
            var movies = _movieService.List(term, true, currentPage);

            // Проверяваме дали няма намерени резултати
            if (movies.MovieList == null || !movies.MovieList.Any())
            {
                // Добавяме съобщение за няма резултати
                ViewData["NoResultsMessage"] = "No movies found matching your search criteria.";
            }

            return View(movies);
        }
            

        public IActionResult About()
        {
            ViewData["Title"] = "About Us"; // Добавено, за да избегнем NullReferenceException
            return View();
        }

        public IActionResult MovieDetail(int movieId)
        {
            var movie = _movieService.GetById(movieId);
            return View(movie);
        }
    }
}
