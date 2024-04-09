using AutoMapper;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.HelperModel;

namespace ExhibitionsService.PL.Mapping.Helper
{
    public class AuthorizationProfiles : Profile
    {
        public AuthorizationProfiles()
        {
            CreateMap<AuthorizationDataModel, AuthorizationDataDTO>().ReverseMap();
            CreateMap<RefreshTokenModel, AuthorizationDataDTO>();
        }
    }
}
