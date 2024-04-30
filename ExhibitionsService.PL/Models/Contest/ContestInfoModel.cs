using ExhibitionsService.PL.Models.Tag;

namespace ExhibitionsService.PL.Models.Contest
{
    public class ContestInfoModel : ContestModel
    {
        public int ConfirmedApplicationsCount { get; set; }
        public int NotConfirmedApplicationsCount { get; set; }

        public List<TagModel> Tags { get; set; } = [];
    }
}
