using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Painter;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    public class PainterController : ControllerBase
    {
        private readonly IPainterService painterService;
        private readonly IMapper mapper;

        public PainterController(IPainterService _painterService, IMapper _mapper)
        {
            painterService = _painterService;
            mapper = _mapper;
        }

        [Route("api/painters")]
        [HttpGet]
        public async Task<IActionResult> GetPainters([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await painterService.GetPagePainterInfoAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<PainterInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<PainterInfoModel>()
                {
                    PageContent = mapper.Map<List<PainterInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("api/painters/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainter(int id)
        {
            return new ObjectResult(ResponseModel<PainterInfoModel>.CoverSuccessResponse(
                mapper.Map<PainterInfoModel>(await painterService.GetByIdWithInfoAsync(id))
                ));
        }

        [Route("api/painters")]
        [HttpPost]
        public async Task<IActionResult> PostPainter([FromBody] PainterCreateModel entity)
        {
            await painterService.CreateAsync(mapper.Map<PainterDTO>(entity));
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }

        [Route("api/painters/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainter(int id, [FromBody] PainterUpdateModel entity)
        {
            if(id != entity.PainterId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await painterService.UpdateAsync(mapper.Map<PainterDTO>(entity));
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }

        [Route("api/painters/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePainter(int id)
        {
            await painterService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }
    }
}
