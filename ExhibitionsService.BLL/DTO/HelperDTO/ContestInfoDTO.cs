namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class ContestInfoDTO : ContestDTO
    {
        public int ConfirmedApplicationsCount { get; set; }
        public int NotConfirmedApplicationsCount { get; set; }

        public List<TagDTO> Tags { get; set; } = [];
    }
}
