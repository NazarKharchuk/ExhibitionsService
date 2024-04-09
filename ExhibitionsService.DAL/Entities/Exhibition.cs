namespace ExhibitionsService.DAL.Entities
{
    public class Exhibition
    {
        public int ExhibitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public bool NeedConfirmation { get; set; }
        public int? PainterLimit { get; set; }

        public ICollection<Tag> Tags { get; set; } = [];

        public ICollection<ExhibitionApplication> Applications { get; set; } = [];
        public ICollection<Painting> Paintings { get; set; } = [];
    }
}
