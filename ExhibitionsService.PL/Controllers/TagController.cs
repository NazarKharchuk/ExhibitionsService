using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Tag;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagController : ControllerBase
    {
        private readonly ITagService tagService;
        private readonly IMapper mapper;

        public TagController(ITagService _tagService, IMapper _mapper)
        {
            tagService = _tagService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTags([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await tagService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<TagModel>>.CoverSuccessResponse(
                new PaginationResponseModel<TagModel>()
                {
                    PageContent = mapper.Map<List<TagModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("~/api/all-tags")]
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            return new ObjectResult(ResponseModel<List<TagModel>>.CoverSuccessResponse(
                mapper.Map<List<TagModel>>(await tagService.GetAllAsync())));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetTag(int id)
        {
            return new ObjectResult(ResponseModel<TagModel>.CoverSuccessResponse(
                mapper.Map<TagModel>(await tagService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostTag([FromBody] TagCreateModel entity)
        {
            await tagService.CreateAsync(mapper.Map<TagDTO>(entity));
            return new ObjectResult(ResponseModel<TagModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutTag(int id, [FromBody] TagUpdateModel entity)
        {
            if (id != entity.TagId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await tagService.UpdateAsync(mapper.Map<TagDTO>(entity));
            return new ObjectResult(ResponseModel<TagModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTag(int id)
        {
            await tagService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<TagModel>.CoverSuccessResponse(null));
        }
    }
}
