namespace ExhibitionsService.DAL.Entities
{
    public class Style
    {
        public int StyleId { get; set; }
        public string StyleName { get; set; }

        public ICollection<Painting> Paintings { get; set; } = [];
    }
}
