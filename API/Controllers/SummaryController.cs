using Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController(ILogger<SummaryController> logger, ISummaryService summaryService) : ControllerBase
    {
        [HttpGet("household/monthly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByMonthAndHouseholdId([FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            var summaries = await summaryService.GetMonthlyTransactionsByMonthAndHouseholdId(financialMonth, requestingUserId);
            return Ok(summaries);
        }

        [HttpGet("household/yearly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByYearAndHouseholdId([FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            var summaries = await summaryService.GetMonthlyTransactionsByYearAndHouseholdId(year, requestingUserId);
            return Ok(summaries);
        }
    }
}
