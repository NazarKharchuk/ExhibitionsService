using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.ExhibitionApplication;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetExhibitionApplications([FromQuery] PaginationRequestModel pagination)
        {
            var paginationResult = await exhibitionApplicationService.GetPageAsync(mapper.Map<PaginationRequestDTO>(pagination));
            return new ObjectResult(ResponseModel<PaginationResponseModel<ExhibitionApplicationModel>>.CoverSuccessResponse(
                new PaginationResponseModel<ExhibitionApplicationModel>()
                {
                    PageContent = mapper.Map<List<ExhibitionApplicationModel>>(paginationResult.Item1),
                    TotalCount = paginationResult.Item2
                }
                ));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetExhibitionApplication(int id)
        {
            return new ObjectResult(ResponseModel<ExhibitionApplicationModel>.CoverSuccessResponse(
                mapper.Map<ExhibitionApplicationModel>(await exhibitionApplicationService.GetByIdAsync(id))
                ));
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "Painter")]
        public async Task<IActionResult> PostExhibitionApplication([FromBody] ExhibitionApplicationCreateModel entity)
        {
            await exhibitionApplicationService.CreateAsync(mapper.Map<ExhibitionApplicationDTO>(entity), HttpContext.User);
            return new ObjectResult(ResponseModel<ExhibitionApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{id}/confirm")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmExhibitionApplication(int id)
        {
            await exhibitionApplicationService.ConfirmApplicationAsync(id);
            return new ObjectResult(ResponseModel<ExhibitionApplicationModel>.CoverSuccessResponse(null));
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Painter, Admin")]
        public async Task<IActionResult> DeleteExhibitionApplication(int id)
        {
            await exhibitionApplicationService.DeleteAsync(id, HttpContext.User);
            return new ObjectResult(ResponseModel<ExhibitionApplicationModel>.CoverSuccessResponse(null));
        }
    }
}
