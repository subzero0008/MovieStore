using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieServices : IMovieService
    {
        private readonly DatabaseContext ctx;
        public MovieServices(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Add(Movie model)
        {
            try
            {
                ctx.Movie.Add(model);
                ctx.SaveChanges();
                foreach (int genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    ctx.MovieGenre.Add(movieGenre);
                }
                ctx.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                var movieGenres = ctx.MovieGenre.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    ctx.MovieGenre.Remove(movieGenre);
                }
                ctx.Movie.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            return ctx.Movie.Find(id);
        }

        public MovieListVm List(string term = "", bool paging = false, int currentPage = 0)
        {
            var data = new MovieListVm();
            var query = ctx.Movie.AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                query = query.Where(a => a.Title.ToLower().StartsWith(term));
            }

            if (paging)
            {
                int pageSize = 5;
                int count = query.Count();
                int totalPages = (int)Math.Ceiling(count / (double)pageSize);
                query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = totalPages;
            }

            var list = query.ToList();
            foreach (var movie in list)
            {
                var genres = (from genre in ctx.Genre
                              join mg in ctx.MovieGenre on genre.Id equals mg.GenreId
                              where mg.MovieId == movie.Id
                              select genre.GenreName).ToList();

                var genreNames = string.Join(',', genres);
                movie.GenreNames = genreNames ?? string.Empty;
            }

            data.MovieList = list.AsQueryable();
            return data;
        }

        public bool Update(Movie model)
        {
            try
            {
                // Актуализиране на жанровете
                var genresToDelete = ctx.MovieGenre.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();
                foreach (var genre in genresToDelete)
                {
                    ctx.MovieGenre.Remove(genre);
                }

                // Добавяне на новите жанрове
                foreach (int genreId in model.Genres)
                {
                    var existingGenre = ctx.MovieGenre.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genreId);
                    if (existingGenre == null)
                    {
                        var movieGenre = new MovieGenre
                        {
                            MovieId = model.Id,
                            GenreId = genreId
                        };
                        ctx.MovieGenre.Add(movieGenre);
                    }
                }

                // Актуализиране на филма (провери дали има промени)
                var existingMovie = ctx.Movie.Find(model.Id);
                if (existingMovie != null)
                {
                    // Обновяване на другите полета на филма (ако има промени)
                    existingMovie.Title = model.Title;
                    existingMovie.ReleaseYear = model.ReleaseYear;
                    existingMovie.MovieImage = model.MovieImage;
                    existingMovie.Cast = model.Cast;
                    existingMovie.Director = model.Director;
                    existingMovie.Synopsis = model.Synopsis;
                    existingMovie.PlaceToShow = model.PlaceToShow;
                }

                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Логиране на грешката, ако има такава
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public List<int> GetGenreByMovieId(int movieId)
        {
            var genreIds = ctx.MovieGenre.Where(a => a.MovieId == movieId).Select(a => a.GenreId).ToList();
            return genreIds;
        }
    }
}
