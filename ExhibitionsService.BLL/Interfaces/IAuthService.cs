using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthorizationDataDTO> LoginAsync(UserProfileDTO entity, IConfiguration _config);
        Task<AuthorizationDataDTO> RefreshTokenAsync(AuthorizationDataDTO entity, IConfiguration _config);
        Task<AuthorizationDataDTO> GetAuthInfoAsync(Claim? profileIdClaim);
    }
}
