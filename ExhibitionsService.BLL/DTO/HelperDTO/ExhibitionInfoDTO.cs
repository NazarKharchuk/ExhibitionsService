namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class ExhibitionInfoDTO : ExhibitionDTO
    {
        public int ConfirmedApplicationsCount { get; set; }
        public int NotConfirmedApplicationsCount { get; set; }

        public List<TagDTO> Tags { get; set; } = [];
    }
}
