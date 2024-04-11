using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Style;

namespace ExhibitionsService.PL.Mapping.Style
{
    public class StyleModelsProfiles : Profile
    {
        public StyleModelsProfiles()
        {
            CreateMap<StyleModel, StyleDTO>().ReverseMap();
            CreateMap<StyleCreateModel, StyleDTO>().ReverseMap();
            CreateMap<StyleUpdateModel, StyleDTO>().ReverseMap();
        }
    }
}
