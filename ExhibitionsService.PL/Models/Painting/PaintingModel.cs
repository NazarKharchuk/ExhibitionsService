using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Painting
{
    public class PaintingModel
    {
        [Required(ErrorMessage = "Поле 'PaintingId' картини є обов'язковим.")]
        public int PaintingId { get; set; }

        [Required(ErrorMessage = "Поле 'Name' картини є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'Name' картини не повинна перевищувати 50 символів.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Description' картини є обов'язковим.")]
        [MaxLength(500, ErrorMessage = "Довжина поля 'Description' картини не повинна перевищувати 500 символів.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'CretionDate' картини є обов'язковим.")]
        public DateTime CretionDate { get; set; }

        [Required(ErrorMessage = "Поле 'Width' картини є обов'язковим.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Поле 'Width' картини повинне бути більшим за 0.")]
        public decimal Width { get; set; }

        [Required(ErrorMessage = "Поле 'Height' картини є обов'язковим.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Поле 'Height' картини повинне бути більшим за 0.")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Поле 'ImagePath' художника є обов'язковим.")]
        [MaxLength(100, ErrorMessage = "Довжина поля 'ImagePath' художника не повинна перевищувати 100 символів.")]
        public string ImagePath { get; set; }

        [MaxLength(100, ErrorMessage = "Довжина поля 'Location' художника не повинна перевищувати 100 символів.")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Поле 'PainterId' картини є обов'язковим.")]
        public int PainterId { get; set; }
    }
}
