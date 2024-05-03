using ExhibitionsService.PL.Models.ContestApplication;
using ExhibitionsService.PL.Models.Painting;

namespace ExhibitionsService.PL.Models.HelperModel
{
    public class ContestApplicationInfoModel : ContestApplicationModel
    {
        public PaintingInfoModel Painting { get; set; }
        public int VotesCount { get; set; }
    }
}
