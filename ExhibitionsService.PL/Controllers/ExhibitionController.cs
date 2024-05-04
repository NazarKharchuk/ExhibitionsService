using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Exhibition;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/exhibitions")]
    public class ExhibitionController : ControllerBase
    {
        private readonly IExhibitionService exhibitionService;
        private readonly IMapper mapper;

        public ExhibitionController(IExhibitionService _exhibitionService, IMapper _mapper)
        {
            exhibitionService = _exhibitionService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetExhibitions([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await exhibitionService.GetPageExhibitionInfoAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<ExhibitionInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ExhibitionInfoModel>()
                {
                    PageContent = mapper.Map<List<ExhibitionInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetExhibition(int id)
        {
            return new ObjectResult(ResponseModel<ExhibitionInfoModel>.CoverSuccessResponse(
                mapper.Map<ExhibitionInfoModel>(await exhibitionService.GetByIdWithInfoAsync(id))
            ));
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostExhibition([FromBody] ExhibitionCreateModel entity)
        {
            var savedModel = await exhibitionService.CreateAsync(mapper.Map<ExhibitionDTO>(entity));
            return new ObjectResult(ResponseModel<ExhibitionModel>.CoverSuccessResponse(mapper.Map<ExhibitionModel>(savedModel)));
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutExhibition(int id, [FromBody] ExhibitionUpdateModel entity)
        {
            if (id != entity.ExhibitionId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await exhibitionService.UpdateAsync(mapper.Map<ExhibitionDTO>(entity));
            return new ObjectResult(ResponseModel<ExhibitionModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            await exhibitionService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<ExhibitionModel>.CoverSuccessResponse(null));
        }

        [Route("{exhibitionId}/tags")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTag(int exhibitionId, [FromBody] int tagId)
        {
            await exhibitionService.AddTagAsync(exhibitionId, tagId);
            return new ObjectResult(ResponseModel<ExhibitionModel>.CoverSuccessResponse(null));
        }

        [Route("{exhibitionId}/tags/{tagId}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTag(int exhibitionId, int tagId)
        {
            await exhibitionService.RemoveTagAsync(exhibitionId, tagId);
            return new ObjectResult(ResponseModel<ExhibitionModel>.CoverSuccessResponse(null));
        }

        [Route("{exhibitionId}/paintings")]
        [HttpGet]
        public async Task<IActionResult> GetPageExhibitionApplicationInfo(int exhibitionId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await exhibitionService.GetPageExhibitionApplicationInfoAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, exhibitionId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ExhibitionApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ExhibitionApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ExhibitionApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{exhibitionId}/not-confirmed-paintings")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPageExhibitionNotConfirmedApplicationInfo(int exhibitionId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await exhibitionService.GetPageExhibitionNotConfirmedApplicationInfoAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, exhibitionId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ExhibitionApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ExhibitionApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ExhibitionApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{exhibitionId}/submissions")]
        [HttpGet]
        [Authorize(Roles = "Painter")]
        public async Task<IActionResult> GetPainterExhibitionSubmissions(int exhibitionId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await exhibitionService.GetPainterExhibitionSubmissionsAsync(
                mapper.Map<PaginationRequestDTO>(pagination),
                HttpContext.User, exhibitionId);
            return new ObjectResult(ResponseModel<PaginationResponseModel<ExhibitionApplicationInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ExhibitionApplicationInfoModel>()
                {
                    PageContent = mapper.Map<List<ExhibitionApplicationInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }
    }
}
