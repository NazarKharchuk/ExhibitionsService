namespace ExhibitionsService.DAL.Entities
{
    public class UserProfile
    {
        public int ProfileId {  get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime JoiningDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Painter? Painter { get; set; }

        public ICollection<PaintingRating> Ratings { get; set; } = [];

        public ICollection<PaintingLike> PaintingLikes { get; set; } = [];
        public ICollection<Painting> LikedPaintings { get; set; } = [];

        public ICollection<ContestApplication> VotedContestApplications { get; set; } = [];
    }
}
