using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.PaintingRating;
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

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingRatings()
        {
            return new ObjectResult(mapper.Map<List<PaintingRatingModel>>((await ratingService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPaintingRating(int id)
        {
            return new ObjectResult(mapper.Map<PaintingRatingModel>(await ratingService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostPaintingRating([FromBody] PaintingRatingCreateModel entity)
        {
            await ratingService.CreateAsync(mapper.Map<PaintingRatingDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPaintingRating(int id, [FromBody] PaintingRatingUpdateModel entity)
        {
            if (id != entity.RatingId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await ratingService.UpdateAsync(mapper.Map<PaintingRatingDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePaintingRating(int id)
        {
            await ratingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
