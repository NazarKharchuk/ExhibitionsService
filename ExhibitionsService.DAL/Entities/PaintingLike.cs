namespace ExhibitionsService.DAL.Entities
{
    public class PaintingLike
    {
        public int ProfileId { get; set; }
        public UserProfile Profile { get; set; }

        public int PaintingId { get; set; }
        public Painting Painting { get; set; }

        public DateTime AddedTime { get; set; }
    }
}
