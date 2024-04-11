using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Material;
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
        public async Task<IActionResult> GetMaterials()
        {
            return new ObjectResult(mapper.Map<List<MaterialModel>>((await materialService.GetAllAsync()).ToList()));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetMaterial(int id)
        {
            return new ObjectResult(mapper.Map<MaterialModel>(await materialService.GetByIdAsync(id)));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostMaterial([FromBody] MaterialCreateModel entity)
        {
            await materialService.CreateAsync(mapper.Map<MaterialDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutMaterial(int id, [FromBody] MaterialUpdateModel entity)
        {
            if (id != entity.MaterialId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await materialService.UpdateAsync(mapper.Map<MaterialDTO>(entity));
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            await materialService.DeleteAsync(id);
            return NoContent();
        }
    }
}
