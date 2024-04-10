using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.ContestApplication;

namespace ExhibitionsService.PL.Mapping.ContestApplication
{
    public class ContestApplicationModelsProfiles : Profile
    {
        public ContestApplicationModelsProfiles()
        {
            CreateMap<ContestApplicationModel, ContestApplicationDTO>().ReverseMap();
            CreateMap<ContestApplicationCreateModel, ContestApplicationDTO>().ReverseMap();
        }
    }
}
