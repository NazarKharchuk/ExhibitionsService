using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.ContestApplication;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/contests_applications")]
    public class ContestApplicationController : ControllerBase
    {
        private readonly IContestApplicationService contestApplicationService;
        private readonly IMapper mapper;

        public ContestApplicationController(IContestApplicationService _contestApplicationService, IMapper _mapper)
        {
            contestApplicationService = _contestApplicationService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetContestApplications()
        {
            return new ObjectResult(mapper.Map<List<ContestApplicationModel>>((await contestApplicationService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetContestApplication(int id)
        {
            return new ObjectResult(mapper.Map<ContestApplicationModel>(await contestApplicationService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostContestApplication([FromBody] ContestApplicationCreateModel entity)
        {
            await contestApplicationService.CreateAsync(mapper.Map<ContestApplicationDTO>(entity));
            return NoContent();
        }

        [Route("{id}/confirm")]
        [HttpPut]
        public async Task<IActionResult> ConfirmContestApplication(int id)
        {
            await contestApplicationService.ConfirmApplicationAsync(id);
            return NoContent();
        }

        [Route("{id}/confirm_winning")]
        [HttpPut]
        public async Task<IActionResult> ConfirmWinningApplication(int id)
        {
            await contestApplicationService.ConfirmWinningAsync(id);
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteContestApplication(int id)
        {
            await contestApplicationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
