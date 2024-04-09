namespace ExhibitionsService.BLL.DTO
{
    public class ExhibitionDTO
    {
        public int ExhibitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public bool NeedConfirmation { get; set; }
        public int? PainterLimit { get; set; }
    }
}
