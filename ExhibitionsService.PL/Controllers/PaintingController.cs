using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Painting;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/paintings")]
    public class PaintingController : ControllerBase
    {
        private readonly IPaintingService paintingService;
        private readonly IMapper mapper;

        public PaintingController(IPaintingService _paintingService, IMapper _mapper)
        {
            paintingService = _paintingService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetPaintings([FromQuery] PaintingsFiltrationPaginationRequestModel filters)
        {
            var paginationResult = await paintingService.GetPagePaintingInfoAsync(
                mapper.Map<PaintingsFiltrationPaginationRequestDTO>(filters),
                HttpContext.User);
            return new ObjectResult(ResponseModel<PaginationResponseModel<PaintingInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<PaintingInfoModel>()
                {
                    PageContent = mapper.Map<List<PaintingInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainting(int id)
        {
            return new ObjectResult(ResponseModel<PaintingInfoModel>.CoverSuccessResponse(
                mapper.Map<PaintingInfoModel>(await paintingService.GetByIdWithInfoAsync(id, HttpContext.User))
                ));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostPainting([FromForm] PaintingCreateModel entity)
        {
            var savedModel = await paintingService.CreateAsync(mapper.Map<PaintingDTO>(entity), entity.Image);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(mapper.Map<PaintingModel>(savedModel)));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainting(int id, [FromForm] PaintingUpdateModel entity)
        {
            if (id != entity.PaintingId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await paintingService.UpdateAsync(mapper.Map<PaintingDTO>(entity), entity.Image);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePainting(int id)
        {
            await paintingService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/genres")]
        [HttpPost]
        public async Task<IActionResult> AddGenre(int paintingId, [FromBody] int genreId)
        {
            await paintingService.AddGenreAsync(paintingId, genreId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/genres/{genreId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveGenre(int paintingId, int genreId)
        {
            await paintingService.RemoveGenreAsync(paintingId, genreId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/styles")]
        [HttpPost]
        public async Task<IActionResult> AddStyle(int paintingId, [FromBody] int styleId)
        {
            await paintingService.AddStyleAsync(paintingId, styleId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/styles/{styleId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveStyle(int paintingId, int styleId)
        {
            await paintingService.RemoveStyleAsync(paintingId, styleId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/materials")]
        [HttpPost]
        public async Task<IActionResult> AddMaterial(int paintingId, [FromBody] int materialId)
        {
            await paintingService.AddMaterialAsync(paintingId, materialId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/materials/{materialId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveMaterial(int paintingId, int materialId)
        {
            await paintingService.RemoveMaterialAsync(paintingId, materialId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/tags")]
        [HttpPost]
        public async Task<IActionResult> AddTag(int paintingId, [FromBody] int tagId)
        {
            await paintingService.AddTagAsync(paintingId, tagId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/tags/{tagId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTag(int paintingId, int tagId)
        {
            await paintingService.RemoveTagAsync(paintingId, tagId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/likes")]
        [HttpPost]
        public async Task<IActionResult> AddLike(int paintingId, [FromBody] int profileId)
        {
            await paintingService.AddLikeAsync(paintingId, profileId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/likes/{profileId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveLike(int paintingId, int profileId)
        {
            await paintingService.RemoveLikeAsync(paintingId, profileId);
            return new ObjectResult(ResponseModel<PaintingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/likes_statistic")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingLikess(int paintingId, [FromQuery] StatisticRequestModel statisticSettings)
        {
            var result = await paintingService.GetLikesStatistics(
                paintingId, statisticSettings.PeriodStart, statisticSettings.PeriodSize);
            return new ObjectResult(ResponseModel<StatisticsResponseModel<StatisticsLikesValueModel>>.CoverSuccessResponse(
                mapper.Map<StatisticsResponseModel<StatisticsLikesValueModel>>(result)));
        }


        [Route("{paintingId}/ratings_statistic")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingRatings(int paintingId, [FromQuery] StatisticRequestModel statisticSettings)
        {
            var result = await paintingService.GetRatingsStatistics(
                paintingId, statisticSettings.PeriodStart, statisticSettings.PeriodSize);
            return new ObjectResult(ResponseModel<StatisticsResponseModel<StatisticsRatingsValueModel>>.CoverSuccessResponse(
                mapper.Map<StatisticsResponseModel<StatisticsRatingsValueModel>>(result)));
        }
    }
}
