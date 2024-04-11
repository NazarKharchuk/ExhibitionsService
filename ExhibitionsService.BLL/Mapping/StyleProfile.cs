using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class StyleProfile : Profile
    {
        public StyleProfile()
        {
            CreateMap<Style, StyleDTO>().ReverseMap();
        }
    }
}
