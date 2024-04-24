using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.UserProfile;

namespace ExhibitionsService.PL.Mapping.UserProfile
{
    public class UserProfilesProfiles : Profile
    {
        public UserProfilesProfiles()
        {
            CreateMap<UserProfileModel, UserProfileDTO>().ReverseMap();
            CreateMap<UserProfileCreateModel, UserProfileDTO>().ReverseMap();
            CreateMap<UserProfileUpdateModel, UserProfileDTO>().ReverseMap();
            CreateMap<LoginModel, UserProfileDTO>().ReverseMap();
            CreateMap<UserProfileInfoDTO, UserProfileInfoModel>();
        }
    }
}
