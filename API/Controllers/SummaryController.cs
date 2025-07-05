using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly ILogger<SummaryController> _logger;
        private readonly ISummaryService _summaryService;

        public SummaryController(ILogger<SummaryController> logger, ISummaryService summaryService)
        {
            _logger = logger;
            _summaryService = summaryService;
        }

        [HttpGet("household/monthly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByMonthAndHouseholdId([FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            var summaries = await _summaryService.GetMonthlyTransactionsByMonthAndHouseholdId(financialMonth, requestingUserId);
            return Ok(summaries);
        }

        [HttpGet("household/yearly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByYearAndHouseholdId([FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            var summaries = await _summaryService.GetMonthlyTransactionsByYearAndHouseholdId(year, requestingUserId);
            return Ok(summaries);
        }
    }
}
