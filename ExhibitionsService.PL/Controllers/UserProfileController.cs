using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService profileService;
        private readonly IMapper mapper;

        public UserProfileController(IUserProfileService _profileService, IMapper _mapper)
        {
            profileService = _profileService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfiles([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await profileService.GetPageUserProfilesAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<UserProfileInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<UserProfileInfoModel>()
                {
                    PageContent = mapper.Map<List<UserProfileInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile(int id)
        {
            return new ObjectResult(ResponseModel<UserProfileModel>.CoverSuccessResponse(
                mapper.Map<UserProfileModel>(await profileService.GetByIdAsync(id, HttpContext.User))
                ));
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutProfile(int id, [FromBody] UserProfileUpdateModel entity)
        {
            if (id != entity.ProfileId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await profileService.UpdateAsync(mapper.Map<UserProfileDTO>(entity), HttpContext.User);
            return new ObjectResult(ResponseModel<UserProfileModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            await profileService.DeleteAsync(id, HttpContext.User);
            return new ObjectResult(ResponseModel<UserProfileModel>.CoverSuccessResponse(null));
        }

        [Route("{id}/admin")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdminRole(int id)
        {
            await profileService.AddRole(id, DAL.Enums.Role.Admin);
            return new ObjectResult(ResponseModel<UserProfileModel>.CoverSuccessResponse(null));
        }

        [Route("{id}/admin")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminRole(int id)
        {
            await profileService.DeleteRole(id, DAL.Enums.Role.Admin);
            return new ObjectResult(ResponseModel<UserProfileModel>.CoverSuccessResponse(null));
        }
    }
}
