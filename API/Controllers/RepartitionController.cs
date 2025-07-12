using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepartitionController(ILogger<RepartitionController> logger, IRepartitionService repartitionService) : ControllerBase
    {
        [HttpGet("household/monthly-repartition")]
        public async Task<IActionResult> GetMonthlyHouseholdByHouseholdId([FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            var repartition = await repartitionService.GetMonthlyHouseholdRepartition(financialMonth, requestingUserId);
            return Ok(repartition);
        }

        [HttpGet("household/yearly-repartition")]
        public async Task<IActionResult> GetYearlyHouseholdByHouseholdId([FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            var repartitions = await repartitionService.GetYearlyHouseholdRepartition( year, requestingUserId);
            return Ok(repartitions);
        }
    }
}
