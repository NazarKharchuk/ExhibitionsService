using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.Painter;

namespace ExhibitionsService.PL.Mapping.Painter
{
    public class PainterModelsProfiles : Profile
    {
        public PainterModelsProfiles()
        {
            CreateMap<PainterModel, PainterDTO>().ReverseMap();
            CreateMap<PainterCreateModel, PainterDTO>().ReverseMap();
            CreateMap<PainterUpdateModel, PainterDTO>().ReverseMap();
            CreateMap<PainterInfoModel, PainterInfoDTO>().ReverseMap();
        }
    }
}
