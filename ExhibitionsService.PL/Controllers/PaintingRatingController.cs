using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.PaintingRating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/painting_ratings")]
    public class PaintingRatingController : ControllerBase
    {
        private readonly IPaintingRatingService ratingService;
        private readonly IMapper mapper;

        public PaintingRatingController(IPaintingRatingService _ratingService, IMapper _mapper)
        {
            ratingService = _ratingService;
            mapper = _mapper;
        }

        [Route("painting/{paintingId}")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingRatings(int paintingId, [FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await ratingService.GetPagePaintingRatingInfoAsync(paintingId, mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<PaintingRatingInfoModel>>.CoverSuccessResponse(
                new PaginationResponseModel<PaintingRatingInfoModel>()
                {
                    PageContent = mapper.Map<List<PaintingRatingInfoModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingRating(int id)
        {
            return new ObjectResult(ResponseModel<PaintingRatingInfoModel>.CoverSuccessResponse(
                mapper.Map<PaintingRatingInfoModel>(await ratingService.GetByIdWithInfoAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostPaintingRating([FromBody] PaintingRatingCreateModel entity)
        {
            await ratingService.CreateAsync(mapper.Map<PaintingRatingDTO>(entity), HttpContext.User);
            return new ObjectResult(ResponseModel<PaintingRatingModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutPaintingRating(int id, [FromBody] PaintingRatingUpdateModel entity)
        {
            if (id != entity.RatingId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await ratingService.UpdateAsync(mapper.Map<PaintingRatingDTO>(entity), HttpContext.User);
            return new ObjectResult(ResponseModel<PaintingRatingModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePaintingRating(int id)
        {
            await ratingService.DeleteAsync(id, HttpContext.User);
            return new ObjectResult(ResponseModel<PaintingRatingModel>.CoverSuccessResponse(null));
        }

        [Route("{paintingId}/my_rating")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyPaintingRating(int paintingId)
        {
            return new ObjectResult(ResponseModel<PaintingRatingInfoModel>.CoverSuccessResponse(
                mapper.Map<PaintingRatingInfoModel>(await ratingService.GetUserPaintingRating(paintingId, HttpContext.User))
                ));
        }
    }
}
