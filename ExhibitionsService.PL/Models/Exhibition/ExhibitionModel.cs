using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Exhibition
{
    public class ExhibitionModel
    {
        [Required(ErrorMessage = "Поле 'ExhibitionId' виставки є обов'язковим.")]
        public int ExhibitionId { get; set; }

        [Required(ErrorMessage = "Поле 'Name' виставки є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'Name' виставки не повинна перевищувати 50 символів.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Description' виставки є обов'язковим.")]
        [MaxLength(500, ErrorMessage = "Довжина поля 'Description' виставки не повинна перевищувати 500 символів.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'AddedDate' виставки є обов'язковим.")]
        public DateTime AddedDate { get; set; }

        [Required(ErrorMessage = "Поле 'NeedConfirmation' виставки є обов'язковим.")]
        public bool NeedConfirmation { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Поле 'PainterLimit' виставки повинне бути більшим за 0.")]
        public int? PainterLimit { get; set; }
    }
}
