using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.ContestApplication;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetContestApplications([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestApplicationService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestApplicationModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestApplicationModel>()
                {
                    PageContent = mapper.Map<List<ContestApplicationModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetContestApplication(int id)
        {
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(
                mapper.Map<ContestApplicationModel>(await contestApplicationService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "Painter")]
        public async Task<IActionResult> PostContestApplication([FromBody] ContestApplicationCreateModel entity)
        {
            await contestApplicationService.CreateAsync(mapper.Map<ContestApplicationDTO>(entity), HttpContext.User);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{id}/confirm")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmContestApplication(int id)
        {
            await contestApplicationService.ConfirmApplicationAsync(id);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{id}/confirm_winning")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmWinningApplication(int id)
        {
            await contestApplicationService.ConfirmWinningAsync(id);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Painter, Admin")]
        public async Task<IActionResult> DeleteContestApplication(int id)
        {
            await contestApplicationService.DeleteAsync(id, HttpContext.User);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{applicationId}/votes")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddVote(int applicationId)
        {
            await contestApplicationService.AddVoteAsync(applicationId, HttpContext.User);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{applicationId}/votes")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveVote(int applicationId)
        {
            await contestApplicationService.RemoveVoteAsync(applicationId, HttpContext.User);
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("determine-winners")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetermineWinners()
        {
            await contestApplicationService.DetermineWinners();
            return new ObjectResult(ResponseModel<ContestApplicationModel>.CoverSuccessResponse(null));
        }
    }
}
