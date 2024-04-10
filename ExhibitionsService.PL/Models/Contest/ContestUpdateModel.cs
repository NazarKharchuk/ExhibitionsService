using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.Contest
{
    public class ContestUpdateModel
    {
        [Required(ErrorMessage = "Поле 'ContestId' конкурсу є обов'язковим.")]
        public int ContestId { get; set; }

        [Required(ErrorMessage = "Поле 'Name' конкурсу є обов'язковим.")]
        [MaxLength(50, ErrorMessage = "Довжина поля 'Name' конкурсу не повинна перевищувати 50 символів.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Description' конкурсу є обов'язковим.")]
        [MaxLength(500, ErrorMessage = "Довжина поля 'Description' конкурсу не повинна перевищувати 500 символів.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'StartDate' конкурсу є обов'язковим.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Поле 'EndDate' конкурсу є обов'язковим.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Поле 'NeedConfirmation' конкурсу є обов'язковим.")]
        public bool NeedConfirmation { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Поле 'PainterLimit' конкурсу повинне бути більшим за 0.")]
        public int? PainterLimit { get; set; }

        [Required(ErrorMessage = "Поле 'WinnersCount' конкурсу є обов'язковим.")]
        [Range(1, int.MaxValue, ErrorMessage = "Поле 'WinnersCount' конкурсу повинне бути більшим за 0.")]
        public int WinnersCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Поле 'VotesLimit' конкурсу повинне бути більшим за 0.")]
        public int? VotesLimit { get; set; }
    }
}
