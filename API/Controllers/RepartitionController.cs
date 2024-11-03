using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RepartitionController : ControllerBase
    {
        private readonly ILogger<RepartitionController> _logger;
        private readonly IRepartitionService _repartitionService;

        public RepartitionController(ILogger<RepartitionController> logger, IRepartitionService repartitionService)
        {
            _logger = logger;
            _repartitionService = repartitionService;
        }

        [HttpGet("household/{householdId}/monthly-repartition")]
        public async Task<IActionResult> GetMonthlyHouseholdByHouseholdId([FromRoute] Guid householdId, [FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || financialMonth == null || !financialMonth.IsFinancialMonthOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, financial month, and requesting user ID are required.");
            }

            try
            {
                var repartition = await _repartitionService.GetMonthlyHouseholdRepartition(householdId, financialMonth, requestingUserId);
                return Ok(repartition);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (HouseholdWithMoreThanTwoUsersNotSupportedException)
            {
                return BadRequest("More than 2 users in the houshold");
            }
            catch (FinancialMonthOfWrongFormatException)
            {
                return BadRequest("Financial month of wrong format exception");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }

        [HttpGet("household/{householdId}/yearly-repartition")]
        public async Task<IActionResult> GetYearlyHouseholdByHouseholdId([FromRoute] Guid householdId, [FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || !year.IsYearOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, financial month, and requesting user ID are required.");
            }

            try
            {
                var repartitions = await _repartitionService.GetYearlyHouseholdRepartition(householdId, year, requestingUserId);
                return Ok(repartitions);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (HouseholdWithMoreThanTwoUsersNotSupportedException)
            {
                return BadRequest("More than 2 users in the houshold");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }
    }
}
