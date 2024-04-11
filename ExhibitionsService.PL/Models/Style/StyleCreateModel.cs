using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Style
{
    public class StyleCreateModel
    {
        [Required(ErrorMessage = "Поле 'StyleName' стилю є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'StyleName' стилю не повинна перевищувати 50 символів.")]
        public string StyleName { get; set; }
    }
}
