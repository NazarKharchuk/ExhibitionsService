using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Painting;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    public class PaintingController : ControllerBase
    {
        private readonly IPaintingService paintingService;
        private readonly IMapper mapper;

        public PaintingController(IPaintingService _paintingService, IMapper _mapper)
        {
            paintingService = _paintingService;
            mapper = _mapper;
        }

        [Route("api/paintings")]
        [HttpGet]
        public async Task<IActionResult> GetPaintings()
        {
            return new ObjectResult(mapper.Map<List<PaintingModel>>((await paintingService.GetAllAsync()).ToList()));
        }

        [Route("api/paintings/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainting(int id)
        {
            return new ObjectResult(mapper.Map<PaintingModel>(await paintingService.GetByIdAsync(id)));
        }

        [Route("api/paintings")]
        [HttpPost]
        public async Task<IActionResult> PostPainting([FromForm] PaintingCreateModel entity)
        {
            await paintingService.CreateAsync(mapper.Map<PaintingDTO>(entity), entity.Image);
            return NoContent();
        }

        [Route("api/paintings/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainting(int id, [FromForm] PaintingUpdateModel entity)
        {
            if (id != entity.PaintingId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await paintingService.UpdateAsync(mapper.Map<PaintingDTO>(entity), entity.Image);
            return NoContent();
        }

        [Route("api/paintings/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePainting(int id)
        {
            await paintingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
