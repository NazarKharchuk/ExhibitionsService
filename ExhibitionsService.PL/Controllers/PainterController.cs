using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    public class PainterController : ControllerBase
    {
        private readonly IPainterService painterService;

        public PainterController(IPainterService _painterService)
        {
            painterService = _painterService;
        }

        [Route("api/painters")]
        [HttpGet]
        public async Task<IActionResult> GetPainters()
        {
            return new ObjectResult((await painterService.GetAllAsync()).ToList());
        }

        [Route("api/painters/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPainter(int id)
        {
            return new ObjectResult(await painterService.GetByIdAsync(id));
        }

        [Route("api/painters")]
        [HttpPost]
        public async Task<IActionResult> PostPainter([FromBody] PainterDTO entity)
        {
            await painterService.CreateAsync(entity);
            return NoContent();
        }

        [Route("api/painters/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutPainter(int id, [FromBody] PainterDTO entity)
        {
            if(id != entity.PainterId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await painterService.UpdateAsync(entity);
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
