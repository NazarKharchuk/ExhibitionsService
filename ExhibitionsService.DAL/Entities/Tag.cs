namespace ExhibitionsService.DAL.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }

        public ICollection<Exhibition> Exhibitions { get; set; } = [];
        public ICollection<Contest> Contests { get; set; } = [];
    }
}
