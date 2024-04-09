using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class ExhibitionProfile : Profile
    {
        public ExhibitionProfile()
        {
            CreateMap<Exhibition, ExhibitionDTO>().ReverseMap();
        }
    }
}
