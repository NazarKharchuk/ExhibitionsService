using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingRatingProfile : Profile
    {
        public PaintingRatingProfile()
        {
            CreateMap<PaintingRating, PaintingRatingDTO>().ReverseMap();
        }
    }
}
