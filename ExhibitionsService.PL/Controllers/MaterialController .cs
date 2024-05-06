using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using ExhibitionsService.PL.Models.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/materials")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService materialService;
        private readonly IMapper mapper;

        public MaterialController(IMaterialService _materialService, IMapper _mapper)
        {
            materialService = _materialService;
            mapper = _mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMaterials([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await materialService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<MaterialModel>>.CoverSuccessResponse(
                new PaginationResponseModel<MaterialModel>()
                {
                    PageContent = mapper.Map<List<MaterialModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("~/api/all-materials")]
        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            return new ObjectResult(ResponseModel<List<MaterialModel>>.CoverSuccessResponse(
                mapper.Map<List<MaterialModel>>(await materialService.GetAllAsync())));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetMaterial(int id)
        {
            return new ObjectResult(ResponseModel<MaterialModel>.CoverSuccessResponse(
                mapper.Map<MaterialModel>(await materialService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostMaterial([FromBody] MaterialCreateModel entity)
        {
            await materialService.CreateAsync(mapper.Map<MaterialDTO>(entity));
            return new ObjectResult(ResponseModel<MaterialModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutMaterial(int id, [FromBody] MaterialUpdateModel entity)
        {
            if (id != entity.MaterialId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await materialService.UpdateAsync(mapper.Map<MaterialDTO>(entity));
            return new ObjectResult(ResponseModel<MaterialModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            await materialService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<MaterialModel>.CoverSuccessResponse(null));
        }
    }
}
