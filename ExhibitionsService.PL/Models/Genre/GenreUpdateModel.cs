using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Genre
{
    public class GenreUpdateModel
    {
        [Required(ErrorMessage = "Поле 'GenreId' жанру є обов'язковим.")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Поле 'GenreName' жанру є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'GenreName' жанру не повинна перевищувати 50 символів.")]
        public string GenreName { get; set; }
    }
}
