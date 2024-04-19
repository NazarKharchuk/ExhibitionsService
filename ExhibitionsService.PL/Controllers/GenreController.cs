using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.Genre;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService genreService;
        private readonly IMapper mapper;

        public GenreController(IGenreService _genreService, IMapper _mapper)
        {
            genreService = _genreService;
            mapper = _mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetGenres([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await genreService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<GenreModel>>.CoverSuccessResponse(
                new PaginationResponseModel<GenreModel>()
                {
                    PageContent = mapper.Map<List<GenreModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGenre(int id)
        {
            return new ObjectResult(ResponseModel<GenreModel>.CoverSuccessResponse(
                mapper.Map<GenreModel>(await genreService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> PostGenre([FromBody] GenreCreateModel entity)
        {
            await genreService.CreateAsync(mapper.Map<GenreDTO>(entity));
            return new ObjectResult(ResponseModel<GenreModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> PutGenre(int id, [FromBody] GenreUpdateModel entity)
        {
            if (id != entity.GenreId)
                throw new ArgumentException("Ідентифікатор, вказаний в URL, не відповідає ідентифікатору у тілі запиту.");

            await genreService.UpdateAsync(mapper.Map<GenreDTO>(entity));
            return new ObjectResult(ResponseModel<GenreModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            await genreService.DeleteAsync(id);
            return new ObjectResult(ResponseModel<GenreModel>.CoverSuccessResponse(null));
        }
    }
}
