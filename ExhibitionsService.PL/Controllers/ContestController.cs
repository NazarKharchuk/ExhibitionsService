using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Contest;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/contests")]
    public class ContestController : ControllerBase
    {
        private readonly IContestService contestService;
        private readonly IMapper mapper;

        public ContestController(IContestService _contestService, IMapper _mapper)
        {
            contestService = _contestService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetContests()
        {
            return new ObjectResult(mapper.Map<List<ContestModel>>((await contestService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetContest(int id)
        {
            return new ObjectResult(mapper.Map<ContestModel>(await contestService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostContest([FromBody] ContestCreateModel entity)
        {
            await contestService.CreateAsync(mapper.Map<ContestDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutContest(int id, [FromBody] ContestUpdateModel entity)
        {
            if (id != entity.ContestId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await contestService.UpdateAsync(mapper.Map<ContestDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteContest(int id)
        {
            await contestService.DeleteAsync(id);
            return NoContent();
        }
    }
}
