using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class ExhibitionApplicationProfile : Profile
    {
        public ExhibitionApplicationProfile()
        {
            CreateMap<ExhibitionApplication, ExhibitionApplicationDTO>().ReverseMap();
        }
    }
}
