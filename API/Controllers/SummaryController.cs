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

        [HttpGet("household/{householdId}/monthly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByMonthAndHouseholdId([FromRoute] Guid householdId, [FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || financialMonth == null || !financialMonth.IsFinancialMonthOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, valid year, and requesting user ID are required.");
            }

            try
            {
                var summaries = await _summaryService.GetMonthlyTransactionsByMonthAndHouseholdId(financialMonth, householdId, requestingUserId);
                return Ok(summaries);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }

        [HttpGet("household/{householdId}/yearly-summaries")]
        public async Task<IActionResult> GetMonthlyTransactionsByYearAndHouseholdId([FromRoute] Guid householdId, [FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || !year.IsYearOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, valid year, and requesting user ID are required.");
            }

            try
            {
                var summaries = await _summaryService.GetMonthlyTransactionsByYearAndHouseholdId(year, householdId, requestingUserId);
                return Ok(summaries);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }
    }
}
