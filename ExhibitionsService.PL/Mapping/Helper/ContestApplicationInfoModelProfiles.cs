using AutoMapper;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.HelperModel;

namespace ExhibitionsService.PL.Mapping.Helper
{
    public class ContestApplicationInfoModelProfiles : Profile
    {
        public ContestApplicationInfoModelProfiles()
        {
            CreateMap<ContestApplicationInfoDTO, ContestApplicationInfoModel>();
        }
    }
}
