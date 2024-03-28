using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PainterProfile : Profile
    {
        public PainterProfile()
        {
            CreateMap<Painter, PainterDTO>().ReverseMap();
        }
    }
}
