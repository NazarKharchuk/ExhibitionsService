using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingRatingProfile : Profile
    {
        public PaintingRatingProfile()
        {
            CreateMap<PaintingRating, PaintingRatingDTO>().ReverseMap();
            CreateMap<PaintingRating, PaintingRatingInfoDTO>()
                .ForMember(dest => dest.AuthorFirstName, opt => opt.MapFrom(src => src.UserProfile.FirstName))
                .ForMember(dest => dest.AuthorLastName, opt => opt.MapFrom(src => src.UserProfile.LastName));
        }
    }
}
