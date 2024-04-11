namespace ExhibitionsService.DAL.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }

        public ICollection<Painting> Paintings { get; set; } = [];
    }
}
