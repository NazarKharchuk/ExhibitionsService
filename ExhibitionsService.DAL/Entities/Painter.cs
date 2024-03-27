namespace ExhibitionsService.DAL.Entities
{
    public class Painter
    {
        public int PainterId { get; set; }
        public string Description { get; set; }
        public string Pseudonym {  get; set; }

        public int ProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
