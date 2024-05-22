using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Enums;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IUserProfileService
    {
        Task CreateAsync(UserProfileDTO entity);
        Task<UserProfileDTO> UpdateAsync(UserProfileDTO entity, ClaimsPrincipal claims);
        Task DeleteAsync(int id, ClaimsPrincipal claims);
        Task<UserProfileDTO?> GetByIdAsync(int id, ClaimsPrincipal claims);
        Task AddRole(int id, Role _role);
        Task DeleteRole(int id, Role _role);
        Task<Tuple<List<UserProfileInfoDTO>, int>> GetPageUserProfilesAsync(PaginationRequestDTO pagination);
    }
}
