namespace ExhibitionsService.BLL.DTO
{
    public class ExhibitionApplicationDTO
    {
        public int ApplicationId { get; set; }
        public bool IsConfirmed { get; set; }

        public int ExhibitionId { get; set; }

        public int PaintingId { get; set; }
    }
}
