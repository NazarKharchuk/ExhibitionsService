using ExhibitionsService.BLL.DTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IUserProfileService
    {
        Task CreateAsync(UserProfileDTO entity);
        Task<UserProfileDTO> UpdateAsync(UserProfileDTO entity);
        Task DeleteAsync(int id);
        Task<UserProfileDTO?> GetByIdAsync(int id);
        //Task<List<UserProfileDTO>> GetAllAsync();
    }
}
