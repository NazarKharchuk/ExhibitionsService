using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class ExhibitionProfile : Profile
    {
        public ExhibitionProfile()
        {
            CreateMap<Exhibition, ExhibitionDTO>().ReverseMap();
            CreateMap<Exhibition, ExhibitionInfoDTO>()
                .ForMember(dest => dest.ConfirmedApplicationsCount, opt => opt.MapFrom(src =>
                    src.NeedConfirmation ? src.Applications.Count(a => a.IsConfirmed) : src.Applications.Count()))
                .ForMember(dest => dest.NotConfirmedApplicationsCount, opt => opt.MapFrom(src =>
                    src.NeedConfirmation ? src.Applications.Count(a => !a.IsConfirmed) : 0))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
        }
    }
}
