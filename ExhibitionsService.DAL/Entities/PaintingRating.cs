namespace ExhibitionsService.DAL.Entities
{
    public class PaintingRating
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set;}
        public string? Comment { get; set;}
        public DateTime AddedDate { get; set; }

        public int ProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public int PaintingId { get; set; }
        public Painting Painting { get; set; }
    }
}
