namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class ContestApplicationInfoDTO : ContestApplicationDTO
    {
        public PaintingInfoDTO Painting { get; set; }
        public int VotesCount { get; set; }
    }
}
