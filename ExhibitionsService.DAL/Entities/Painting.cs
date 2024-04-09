namespace ExhibitionsService.DAL.Entities
{
    public class Painting
    {
        public int PaintingId { get; set; }
        public string Name { get; set;}
        public string Description { get; set;}
        public DateTime CretionDate { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string ImagePath { get; set; }
        public string? Location { get; set; }

        public int PainterId { get; set; }
        public Painter Painter { get; set; }

        public ICollection<PaintingRating> Ratings { get; set; } = [];
    }
}
