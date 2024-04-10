namespace ExhibitionsService.DAL.Entities
{
    public class ContestApplication
    {
        public int ApplicationId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsWon { get; set; }

        public int ContestId { get; set; }
        public Contest Contest { get; set; }

        public int PaintingId { get; set; }
        public Painting Painting { get; set; }
    }
}
