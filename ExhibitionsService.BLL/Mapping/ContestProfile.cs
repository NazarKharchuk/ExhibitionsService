using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class ContestProfile : Profile
    {
        public ContestProfile()
        {
            CreateMap<Contest, ContestDTO>().ReverseMap();
        }
    }
}
