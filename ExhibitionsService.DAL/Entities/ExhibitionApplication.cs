namespace ExhibitionsService.DAL.Entities
{
    public class ExhibitionApplication
    {
        public int ApplicationId { get; set; }
        public bool IsConfirmed { get; set; }

        public int ExhibitionId { get; set; }
        public Exhibition Exhibition { get; set; }

        public int PaintingId { get; set; }
        public Painting Painting { get; set; }
    }
}
