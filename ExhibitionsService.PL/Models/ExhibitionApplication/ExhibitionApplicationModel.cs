using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.ExhibitionApplication
{
    public class ExhibitionApplicationModel
    {
        [Required(ErrorMessage = "Поле 'ApplicationId' заявки на участь у виставці є обов'язковим.")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Поле 'IsConfirmed' заявки на участь у виставці є обов'язковим.")]
        public bool IsConfirmed { get; set; }

        [Required(ErrorMessage = "Поле 'ExhibitionId' заявки на участь у виставці є обов'язковим.")]
        public int ExhibitionId { get; set; }

        [Required(ErrorMessage = "Поле 'PaintingId' заявки на участь у виставці є обов'язковим.")]
        public int PaintingId { get; set; }
    }
}
