using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.ExhibitionApplication;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/exhibitions_applications")]
    public class ExhibitionApplicationController : ControllerBase
    {
        private readonly IExhibitionApplicationService exhibitionApplicationService;
        private readonly IMapper mapper;

        public ExhibitionApplicationController(IExhibitionApplicationService _exhibitionApplicationService, IMapper _mapper)
        {
            exhibitionApplicationService = _exhibitionApplicationService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetExhibitionApplications()
        {
            return new ObjectResult(mapper.Map<List<ExhibitionApplicationModel>>((await exhibitionApplicationService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetExhibitionApplication(int id)
        {
            return new ObjectResult(mapper.Map<ExhibitionApplicationModel>(await exhibitionApplicationService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostExhibitionApplication([FromBody] ExhibitionApplicationCreateModel entity)
        {
            await exhibitionApplicationService.CreateAsync(mapper.Map<ExhibitionApplicationDTO>(entity));
            return NoContent();
        }

        [Route("{id}/confirm")]
        [HttpPut]
        public async Task<IActionResult> ConfirmExhibitionApplication(int id)
        {
            await exhibitionApplicationService.ConfirmApplicationAsync(id);
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteExhibitionApplication(int id)
        {
            await exhibitionApplicationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
