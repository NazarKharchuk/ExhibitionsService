using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingProfile : Profile
    {
        public PaintingProfile()
        {
            CreateMap<Painting, PaintingDTO>().ReverseMap();
            CreateMap<Painting, PaintingInfoDTO>()
                .ForMember(dest => dest.Painter, opt => opt.MapFrom(src => src.Painter))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
                .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.Styles))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => src.Materials))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        }
    }
}
