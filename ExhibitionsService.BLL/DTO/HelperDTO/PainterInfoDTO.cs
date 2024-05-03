namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class PainterInfoDTO : PainterDTO
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime JoiningDate { get; set; }
        public int LikesCount { get; set; }
        public int VictoriesCount { get; set; }
        public int RatingCount { get; set; }
        public decimal AvgRating { get; set; }
    }
}
