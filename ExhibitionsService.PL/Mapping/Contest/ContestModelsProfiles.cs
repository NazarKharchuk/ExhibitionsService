using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Contest;

namespace ExhibitionsService.PL.Mapping.Contest
{
    public class ContestModelsProfiles : Profile
    {
        public ContestModelsProfiles()
        {
            CreateMap<ContestModel, ContestDTO>().ReverseMap();
            CreateMap<ContestCreateModel, ContestDTO>().ReverseMap();
            CreateMap<ContestUpdateModel, ContestDTO>().ReverseMap();
        }
    }
}
