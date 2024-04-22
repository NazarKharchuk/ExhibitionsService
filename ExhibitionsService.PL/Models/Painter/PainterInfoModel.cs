namespace ExhibitionsService.PL.Models.Painter
{
    public class PainterInfoModel
    {
        public int PainterId { get; set; }
        public string Pseudonym { get; set; }
        public string Description { get; set; }
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime JoiningDate { get; set; }
        public int LikesCount { get; set; }
        public int VictoriesCount { get; set; }
        public int RatingCount { get; set; }
        public decimal AvgRating { get; set; }
    }
}
