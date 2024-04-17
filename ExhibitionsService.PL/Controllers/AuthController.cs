using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserProfileService profileService;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public AuthController(IAuthService _authService, IUserProfileService _profileService, IMapper _mapper, IConfiguration _config)
        {
            authService = _authService;
            profileService = _profileService;
            mapper = _mapper;
            config = _config;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserProfileCreateModel entity)
        {
            await profileService.CreateAsync(mapper.Map<UserProfileDTO>(entity));
            return NoContent();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel entity)
        {
            var res = await authService.LoginAsync(mapper.Map<UserProfileDTO>(entity), config);
            return new ObjectResult(mapper.Map<AuthorizationDataModel>(res));
        }

        [Route("refresh")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel entity)
        {
            var res = await authService.RefreshTokenAsync(mapper.Map<AuthorizationDataDTO>(entity), config);
            return new ObjectResult(mapper.Map<AuthorizationDataModel>(res));
        }

        [Authorize]
        [Route("get_info")]
        [HttpGet]
        public async Task<IActionResult> GetAuthInfo()
        {
            Claim? profileIdClaim = HttpContext.User.FindFirst("ProfileId");
            var res = await authService.GetAuthInfoAsync(profileIdClaim);
            return new ObjectResult(mapper.Map<AuthorizationDataModel>(res));
        }
    }
}
