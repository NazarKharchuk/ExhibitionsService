namespace ExhibitionsService.BLL.DTO
{
    public class ContestApplicationDTO
    {
        public int ApplicationId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsWon { get; set; }

        public int ContestId { get; set; }

        public int PaintingId { get; set; }
    }
}
