using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.PL.Models.HelperModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExhibitionsService.PL.Controllers
{
    [ApiController]
    [Route("api/paintings/purchase")]
    [Authorize]
    public class PaintingBuyingController : ControllerBase
    {
        private readonly IPaintingBuyingService buyingService;
        private readonly IConfiguration config;

        public PaintingBuyingController(IPaintingBuyingService _buyingService, IConfiguration _config)
        {
            buyingService = _buyingService;
            config = _config;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> BuyPainting([FromBody] int paintingId)
        {
            var sessionId = await buyingService.BuyPainting(paintingId, HttpContext.Request.Headers["Origin"].ToString());
            return new ObjectResult(ResponseModel<PaintingBuyingResponseModel>.CoverSuccessResponse(new PaintingBuyingResponseModel()
            {
                SessionId = sessionId,
                StripePublishableKey = config["Stripe:PublishableKey"]
            }));
        }

        [Route("success")]
        [HttpPost]
        public async Task<IActionResult> ProcessSuccessfulBuying([FromBody] string sessionId)
        {
            await buyingService.ProcessSuccessfulBuying(sessionId);
            return new ObjectResult(ResponseModel<string>.CoverSuccessResponse(null));
        }
    }
}
