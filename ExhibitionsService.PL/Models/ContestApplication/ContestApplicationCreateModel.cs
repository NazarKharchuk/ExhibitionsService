using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.ContestApplication
{
    public class ContestApplicationCreateModel
    {
        [Required(ErrorMessage = "Поле 'ContestId' заявки на участь у конкурсі є обов'язковим.")]
        public int ContestId { get; set; }

        [Required(ErrorMessage = "Поле 'PaintingId' заявки на участь у конкурсі є обов'язковим.")]
        public int PaintingId { get; set; }
    }
}
