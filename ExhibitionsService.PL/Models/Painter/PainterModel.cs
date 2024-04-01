using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Painter
{
    public class PainterModel
    {
        [Required(ErrorMessage = "Поле 'PainterId' художника є обов'язковим.")]
        public int PainterId { get; set; }

        [Required(ErrorMessage = "Поле 'Description' художника є обов'язковим.")]
        [MaxLength(500, ErrorMessage = "Довжина поля 'Description' художника не повинна перевищувати 500 символів.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'Pseudonym' художника є обов'язковим.")]
        [MaxLength(20, ErrorMessage = "Довжина поля 'Pseudonym' художника не повинна перевищувати 20 символів.")]
        public string Pseudonym { get; set; }

        [Required(ErrorMessage = "Поле 'ProfileId' художника є обов'язковим.")]
        public int ProfileId { get; set; }
    }
}
