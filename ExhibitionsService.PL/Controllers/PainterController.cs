using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
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
        public async Task<IActionResult> GetPainters()
        {
            return new ObjectResult(mapper.Map<List<PainterModel>>((await painterService.GetAllAsync()).ToList()));
        }

        [Route("api/painters/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainter(int id)
        {
            return new ObjectResult(mapper.Map<PainterModel>(await painterService.GetByIdAsync(id)));
        }

        [Route("api/painters")]
        [HttpPost]
        public async Task<IActionResult> PostPainter([FromBody] PainterCreateModel entity)
        {
            await painterService.CreateAsync(mapper.Map<PainterDTO>(entity));
            return NoContent();
        }

        [Route("api/painters/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainter(int id, [FromBody] PainterUpdateModel entity)
        {
            if(id != entity.PainterId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await painterService.UpdateAsync(mapper.Map<PainterDTO>(entity));
            return NoContent();
        }

        [Route("api/painters/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePainter(int id)
        {
            await painterService.DeleteAsync(id);
            return NoContent();
        }
    }
}
