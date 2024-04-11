namespace ExhibitionsService.DAL.Entities
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }

        public ICollection<Painting> Paintings { get; set; } = [];
    }
}
