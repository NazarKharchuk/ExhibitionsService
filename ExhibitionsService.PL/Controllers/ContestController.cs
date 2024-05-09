using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Contest;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/contests")]
    public class ContestController : ControllerBase
    {
        private readonly IContestService contestService;
        private readonly IMapper mapper;

        public ContestController(IContestService _contestService, IMapper _mapper)
        {
            contestService = _contestService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetContests([FromQuery] ContestsFiltrationPaginationRequestModel filters)
        {
            var paginationResult = await contestService.GetPageContestInfoAsync(mapper.Map<ContestsFiltrationPaginationRequestDTO>(filters));
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestInfoModel>()
                {
                    PageContent = mapper.Map<List<ContestInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetContest(int id)
        {
            return new ObjectResult(ResponseModel<ContestInfoModel>.CoverSuccessResponse(
                mapper.Map<ContestInfoModel>(await contestService.GetByIdWithInfoAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostContest([FromBody] ContestCreateModel entity)
        {
            var savedModel = await contestService.CreateAsync(mapper.Map<ContestDTO>(entity));
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(mapper.Map<ContestModel>(savedModel)));
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutContest(int id, [FromBody] ContestUpdateModel entity)
        {
            if (id != entity.ContestId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await contestService.UpdateAsync(mapper.Map<ContestDTO>(entity));
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContest(int id)
        {
            await contestService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{contestId}/tags")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTag(int contestId, [FromBody] int tagId)
        {
            await contestService.AddTagAsync(contestId, tagId);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{contestId}/tags/{tagId}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTag(int contestId, int tagId)
        {
            await contestService.RemoveTagAsync(contestId, tagId);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{contestId}/paintings")]
        [HttpGet]
        public async Task<IActionResult> GetPageContestApplicationInfo(int contestId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestService.GetPageContestApplicationInfoAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, contestId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ContestApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{contestId}/not-confirmed-paintings")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPageContestNotConfirmedApplicationInfo(int contestId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestService.GetPageContestNotConfirmedApplicationInfoAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, contestId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ContestApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{contestId}/submissions")]
        [HttpGet]
        [Authorize(Roles = "Painter")]
        public async Task<IActionResult> GetPainterContestSubmissions(int contestId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestService.GetPainterContestSubmissionsAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, contestId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ContestApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{contestId}/votes")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserContestVotes(int contestId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestService.GetUserContestVotesAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, contestId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ContestApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ContestApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ContestApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }
    }
}
