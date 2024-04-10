using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.ContestApplication
{
    public class ContestApplicationModel
    {
        [Required(ErrorMessage = "Поле 'ApplicationId' заявки на участь у конкурсі є обов'язковим.")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Поле 'IsConfirmed' заявки на участь у конкурсі є обов'язковим.")]
        public bool IsConfirmed { get; set; }

        [Required(ErrorMessage = "Поле 'IsWon' заявки на участь у конкурсі є обов'язковим.")]
        public bool IsWon { get; set; }

        [Required(ErrorMessage = "Поле 'ContestId' заявки на участь у конкурсі є обов'язковим.")]
        public int ContestId { get; set; }

        [Required(ErrorMessage = "Поле 'PaintingId' заявки на участь у конкурсі є обов'язковим.")]
        public int PaintingId { get; set; }
    }
}
