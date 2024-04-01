using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Tag
{
    public class TagUpdateModel
    {
        [Required(ErrorMessage = "Поле 'TagId' тега є обов'язковим.")]
        public int TagId { get; set; }

        [Required(ErrorMessage = "Поле 'TagName' тега є обов'язковим.")]
        [MaxLength(20, ErrorMessage = "Довжина поля 'TagName' тега не повинна перевищувати 20 символів.")]
        public string TagName { get; set; }
    }
}
