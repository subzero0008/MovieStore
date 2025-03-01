using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStoreMvc.Models.Domain
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required")]
        public string Title { get; set; } = string.Empty;

        public string? ReleaseYear { get; set; }

        public string? MovieImage { get; set; }

        [Required(ErrorMessage = "The cast is required")]
        public string? Cast { get; set; } = string.Empty;

        [Required(ErrorMessage = "The director is required")]
        public string? Director { get; set; } = string.Empty;

        public string? Synopsis { get; set; } = string.Empty;

        public string? PlaceToShow { get; set; } = string.Empty;

        // Премахваме Required атрибута, за да не бъде задължително при редактиране
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Премахваме Required атрибута за Genres, за да не бъде задължително при редактиране
        [NotMapped]
        public List<int>? Genres { get; set; } = new List<int>();

        [NotMapped]
        public IEnumerable<SelectListItem>? GenreList { get; set; } = new List<SelectListItem>();

        [NotMapped]
        public string? GenreNames { get; set; } = string.Empty;

        [NotMapped]
        public MultiSelectList? MultiGenreList { get; set; }
    }
}
