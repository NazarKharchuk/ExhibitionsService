using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.ExhibitionApplication
{
    public class ExhibitionApplicationCreateModel
    {
        [Required(ErrorMessage = "Поле 'ExhibitionId' заявки на участь у виставці є обов'язковим.")]
        public int ExhibitionId { get; set; }

        [Required(ErrorMessage = "Поле 'PaintingId' заявки на участь у виставці є обов'язковим.")]
        public int PaintingId { get; set; }
    }
}
