using ExhibitionsService.PL.Models.ExhibitionApplication;
using ExhibitionsService.PL.Models.Painting;

namespace ExhibitionsService.PL.Models.HelperModel
{
    public class ExhibitionApplicationInfoModel : ExhibitionApplicationModel
    {
        public PaintingInfoModel Painting { get; set; }
    }
}
