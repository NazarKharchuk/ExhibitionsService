using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Material
{
    public class MaterialModel
    {
        [Required(ErrorMessage = "Поле 'MaterialId' матеріалу є обов'язковим.")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Поле 'MaterialName' матеріалу є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'MaterialName' матеріалу не повинна перевищувати 50 символів.")]
        public string MaterialName { get; set; }
    }
}
