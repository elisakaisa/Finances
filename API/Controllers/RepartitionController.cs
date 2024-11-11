using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("household/monthly-repartition")]
        public async Task<IActionResult> GetMonthlyHouseholdByHouseholdId([FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            if (financialMonth == null || !financialMonth.IsFinancialMonthOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Financial month, and requesting user ID are required.");
            }

            try
            {
                var repartition = await _repartitionService.GetMonthlyHouseholdRepartition(financialMonth, requestingUserId);
                return Ok(repartition);
            }
            catch (HouseholdWithMoreThanTwoUsersNotSupportedException)
            {
                return BadRequest("More than 2 users in the houshold");
            }
            catch (FinancialMonthOfWrongFormatException)
            {
                return BadRequest("Financial month of wrong format exception");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }

        [HttpGet("household/yearly-repartition")]
        public async Task<IActionResult> GetYearlyHouseholdByHouseholdId([FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            if (!year.IsYearOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Year and requesting user ID are required.");
            }

            try
            {
                var repartitions = await _repartitionService.GetYearlyHouseholdRepartition( year, requestingUserId);
                return Ok(repartitions);
            }
            catch (HouseholdWithMoreThanTwoUsersNotSupportedException)
            {
                return BadRequest("More than 2 users in the household");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }
    }
}
