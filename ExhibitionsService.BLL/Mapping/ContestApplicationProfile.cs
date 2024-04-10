using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class ContestApplicationProfile : Profile
    {
        public ContestApplicationProfile()
        {
            CreateMap<ContestApplication, ContestApplicationDTO>().ReverseMap();
        }
    }
}
