using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Style;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/styles")]
    public class StyleController : ControllerBase
    {
        private readonly IStyleService styleService;
        private readonly IMapper mapper;

        public StyleController(IStyleService _styleService, IMapper _mapper)
        {
            styleService = _styleService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetStyles([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await styleService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<StyleModel>>.CoverSuccessResponse(
                new PaginationResponseModel<StyleModel>()
                {
                    PageContent = mapper.Map<List<StyleModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("~/api/all-styles")]
        [HttpGet]
        public async Task<IActionResult> GetAllStyles()
        {
            return new ObjectResult(ResponseModel<List<StyleModel>>.CoverSuccessResponse(
                mapper.Map<List<StyleModel>>(await styleService.GetAllAsync())));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetStyle(int id)
        {
            return new ObjectResult(ResponseModel<StyleModel>.CoverSuccessResponse(
                mapper.Map<StyleModel>(await styleService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostStyle([FromBody] StyleCreateModel entity)
        {
            await styleService.CreateAsync(mapper.Map<StyleDTO>(entity));
            return new ObjectResult(ResponseModel<StyleModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutStyle(int id, [FromBody] StyleUpdateModel entity)
        {
            if (id != entity.StyleId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await styleService.UpdateAsync(mapper.Map<StyleDTO>(entity));
            return new ObjectResult(ResponseModel<StyleModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteStyle(int id)
        {
            await styleService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<StyleModel>.CoverSuccessResponse(null));
        }
    }
}
