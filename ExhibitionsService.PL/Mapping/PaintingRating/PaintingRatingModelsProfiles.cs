using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.PaintingRating;

namespace ExhibitionsService.PL.Mapping.PaintingRating
{
    public class PaintingRatingModelsProfiles : Profile
    {
        public PaintingRatingModelsProfiles()
        {
            CreateMap<PaintingRatingModel, PaintingRatingDTO>().ReverseMap();
            CreateMap<PaintingRatingCreateModel, PaintingRatingDTO>().ReverseMap();
            CreateMap<PaintingRatingUpdateModel, PaintingRatingDTO>().ReverseMap();
        }
    }
}
