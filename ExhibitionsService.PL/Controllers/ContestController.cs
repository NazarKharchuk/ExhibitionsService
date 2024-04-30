using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Contest;
using ExhibitionsService.PL.Models.HelperModel;
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
        public async Task<IActionResult> GetContests([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await contestService.GetPageContestInfoAsync(mapper.Map<PaginationRequestDTO>(pagination));
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
        public async Task<IActionResult> PostContest([FromBody] ContestCreateModel entity)
        {
            var savedModel = await contestService.CreateAsync(mapper.Map<ContestDTO>(entity));
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(mapper.Map<ContestModel>(savedModel)));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutContest(int id, [FromBody] ContestUpdateModel entity)
        {
            if (id != entity.ContestId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await contestService.UpdateAsync(mapper.Map<ContestDTO>(entity));
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteContest(int id)
        {
            await contestService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{contestId}/tags")]
        [HttpPost]
        public async Task<IActionResult> AddTag(int contestId, [FromBody] int tagId)
        {
            await contestService.AddTagAsync(contestId, tagId);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }

        [Route("{contestId}/tags/{tagId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTag(int contestId, int tagId)
        {
            await contestService.RemoveTagAsync(contestId, tagId);
            return new ObjectResult(ResponseModel<ContestModel>.CoverSuccessResponse(null));
        }
    }
}
