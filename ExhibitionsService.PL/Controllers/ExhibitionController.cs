using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Exhibition;
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
        public async Task<IActionResult> GetExhibitions()
        {
            return new ObjectResult(mapper.Map<List<ExhibitionModel>>((await exhibitionService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetExhibition(int id)
        {
            return new ObjectResult(mapper.Map<ExhibitionModel>(await exhibitionService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostExhibition([FromBody] ExhibitionCreateModel entity)
        {
            await exhibitionService.CreateAsync(mapper.Map<ExhibitionDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutExhibition(int id, [FromBody] ExhibitionUpdateModel entity)
        {
            if (id != entity.ExhibitionId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await exhibitionService.UpdateAsync(mapper.Map<ExhibitionDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            await exhibitionService.DeleteAsync(id);
            return NoContent();
        }
    }
}
