using ExhibitionsService.PL.Models.Tag;

namespace ExhibitionsService.PL.Models.Exhibition
{
    public class ExhibitionInfoModel : ExhibitionModel
    {
        public int ConfirmedApplicationsCount { get; set; }
        public int NotConfirmedApplicationsCount { get; set; }

        public List<TagModel> Tags { get; set; } = [];
    }
}
