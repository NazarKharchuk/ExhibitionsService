using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Tag;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService tagService;
        private readonly IMapper mapper;

        public TagController(ITagService _tagService, IMapper _mapper)
        {
            tagService = _tagService;
            mapper = _mapper;
        }

        [Route("api/tags")]
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return new ObjectResult(mapper.Map<List<TagModel>>((await tagService.GetAllAsync()).ToList()));
        }

        [Route("api/tags/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetTag(int id)
        {
            return new ObjectResult(mapper.Map<TagModel>(await tagService.GetByIdAsync(id)));
        }

        [Route("api/tags")]
        [HttpPost]
        public async Task<IActionResult> PostTag([FromBody] TagCreateModel entity)
        {
            await tagService.CreateAsync(mapper.Map<TagDTO>(entity));
            return NoContent();
        }

        [Route("api/tags/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutTag(int id, [FromBody] TagUpdateModel entity)
        {
            if (id != entity.TagId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await tagService.UpdateAsync(mapper.Map<TagDTO>(entity));
            return NoContent();
        }

        [Route("api/tags/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTag(int id)
        {
            await tagService.DeleteAsync(id);
            return NoContent();
        }
    }
}
