using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.PaintingRating
{
    public class PaintingRatingCreateModel
    {
        [Required(ErrorMessage = "Поле 'RatingValue' відгуку є обов'язковим.")]
        [Range(0, 5, ErrorMessage = "Поле 'RatingValue' відгуку повинне бути більшим за 0 та меншим за 5.")]
        public decimal RatingValue { get; set; }

        [MaxLength(500, ErrorMessage = "Довжина поля 'Comment' відгуку не повинна перевищувати 500 символів.")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Поле 'ProfileId' відгуку є обов'язковим.")]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "Поле 'PaintingId' відгуку є обов'язковим.")]
        public int PaintingId { get; set; }
    }
}
