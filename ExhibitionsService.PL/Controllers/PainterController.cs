using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.BLL.Services;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Painter;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/painters")]
    public class PainterController : ControllerBase
    {
        private readonly IPainterService painterService;
        private readonly IMapper mapper;

        public PainterController(IPainterService _painterService, IMapper _mapper)
        {
            painterService = _painterService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetPainters([FromQuery] PaintersFiltrationPaginationRequestModel filters)
        {
            var paginationResult = await painterService.GetPagePainterInfoAsync(mapper.Map<PaintersFiltrationPaginationRequestDTO>(filters));
            return new ObjectResult(ResponseModel<PaginationResponseModel<PainterInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<PainterInfoModel>()
                {
                    PageContent = mapper.Map<List<PainterInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainter(int id)
        {
            return new ObjectResult(ResponseModel<PainterInfoModel>.CoverSuccessResponse(
                mapper.Map<PainterInfoModel>(await painterService.GetByIdWithInfoAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostPainter([FromBody] PainterCreateModel entity)
        {
            await painterService.CreateAsync(mapper.Map<PainterDTO>(entity));
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainter(int id, [FromBody] PainterUpdateModel entity)
        {
            if(id != entity.PainterId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await painterService.UpdateAsync(mapper.Map<PainterDTO>(entity));
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePainter(int id)
        {
            await painterService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<PainterModel>.CoverSuccessResponse(null));
        }

        [Route("{painterId}/likes_statistic")]
        [HttpGet]
        public async Task<IActionResult> GetPainterLikess(int painterId, [FromQuery] StatisticRequestModel statisticSettings)
        {
            var result = await painterService.GetLikesStatistics(
                painterId, statisticSettings.PeriodStart, statisticSettings.PeriodSize);
            return new ObjectResult(ResponseModel<StatisticsResponseModel<StatisticsLikesValueModel>>.CoverSuccessResponse(
                mapper.Map<StatisticsResponseModel<StatisticsLikesValueModel>>(result)));
        }


        [Route("{painterId}/ratings_statistic")]
        [HttpGet]
        public async Task<IActionResult> GetPainterRatings(int painterId, [FromQuery] StatisticRequestModel statisticSettings)
        {
            var result = await painterService.GetRatingsStatistics(
                painterId, statisticSettings.PeriodStart, statisticSettings.PeriodSize);
            return new ObjectResult(ResponseModel<StatisticsResponseModel<StatisticsRatingsValueModel>>.CoverSuccessResponse(
                mapper.Map<StatisticsResponseModel<StatisticsRatingsValueModel>>(result)));
        }
    }
}
