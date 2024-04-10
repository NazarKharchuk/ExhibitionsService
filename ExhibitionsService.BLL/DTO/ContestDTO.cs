namespace ExhibitionsService.BLL.DTO
{
    public class ContestDTO
    {
        public int ContestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool NeedConfirmation { get; set; }
        public int? PainterLimit { get; set; }
        public int WinnersCount { get; set; }
        public int? VotesLimit { get; set; }
    }
}
