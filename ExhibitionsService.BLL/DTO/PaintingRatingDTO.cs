namespace ExhibitionsService.BLL.DTO
{
    public class PaintingRatingDTO
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime AddedDate { get; set; }

        public int ProfileId { get; set; }

        public int PaintingId { get; set; }
    }
}
