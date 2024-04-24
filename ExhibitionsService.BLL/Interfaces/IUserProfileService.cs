using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Enums;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IUserProfileService
    {
        Task CreateAsync(UserProfileDTO entity);
        Task<UserProfileDTO> UpdateAsync(UserProfileDTO entity);
        Task DeleteAsync(int id);
        Task<UserProfileDTO?> GetByIdAsync(int id);
        Task AddRole(int id, Role _role);
        Task DeleteRole(int id, Role _role);
        Task<Tuple<List<UserProfileInfoDTO>, int>> GetPageUserProfilesAsync(PaginationRequestDTO pagination);
    }
}
