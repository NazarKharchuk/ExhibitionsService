using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.UserProfile;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService profileService;
        private readonly IMapper mapper;

        public UserProfileController(IUserProfileService _profileService, IMapper _mapper)
        {
            profileService = _profileService;
            mapper = _mapper;
        }

        /*[Route("api/profiles")]
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            return new ObjectResult(mapper.Map<List<UserProfileModel>>((await profileService.GetAllAsync()).ToList()));
        }*/

        [Route("api/profiles/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetProfile(int id)
        {
            return new ObjectResult(mapper.Map<UserProfileModel>(await profileService.GetByIdAsync(id)));
        }

        /*[Route("api/profiles")]
        [HttpPost]
        public async Task<IActionResult> PostProfile([FromBody] UserProfileCreateModel entity)
        {
            await profileService.CreateAsync(mapper.Map<UserProfileDTO>(entity));
            return NoContent();
        }*/

        [Route("api/profiles/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutProfile(int id, [FromBody] UserProfileUpdateModel entity)
        {
            if (id != entity.ProfileId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await profileService.UpdateAsync(mapper.Map<UserProfileDTO>(entity));
            return NoContent();
        }

        [Route("api/profiles/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            await profileService.DeleteAsync(id);
            return NoContent();
        }
    }
}
