using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.PaintingRating
{
    public class PaintingRatingUpdateModel
    {
        [Required(ErrorMessage = "Поле 'RatingId' відгуку є обов'язковим.")]
        public int RatingId { get; set; }

        [Required(ErrorMessage = "Поле 'RatingValue' відгуку є обов'язковим.")]
        [Range(0, 5, ErrorMessage = "Поле 'RatingValue' відгуку повинне бути більшим за 0 та меншим за 5.")]
        public decimal RatingValue { get; set; }

        [MaxLength(500, ErrorMessage = "Довжина поля 'Comment' відгуку не повинна перевищувати 500 символів.")]
        public string? Comment { get; set; }
    }
}
