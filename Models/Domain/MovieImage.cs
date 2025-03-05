using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStoreMvc.Models.Domain
{
    public class MovieImage
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        // Правим ImagePath nullable, защото може да няма изображение
        public string? ImagePath { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; } // Навигационен обект към Movie
    }
}
