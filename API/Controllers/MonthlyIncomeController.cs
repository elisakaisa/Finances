using Common.Model.Dtos;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyIncomeController : ControllerBase
    {
        private readonly ILogger<IMonthlyIncomeAfterTaxesService> _logger;
        private readonly IMonthlyIncomeAfterTaxesService _monthlyIncomeAfterTaxesService;

        public MonthlyIncomeController(ILogger<IMonthlyIncomeAfterTaxesService> logger, IMonthlyIncomeAfterTaxesService monthlyIncomeAfterTaxesService)
        {
            _logger = logger;
            _monthlyIncomeAfterTaxesService = monthlyIncomeAfterTaxesService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMonthlyIncomeAfterTax([FromBody] MonthlyIncomeAfterTaxDto monthlyIncome, [FromHeader] Guid userId)
        {
            if (monthlyIncome == null || userId == Guid.Empty)
            {
                return BadRequest("Transaction data or user ID is missing.");
            }

            try
            {
                var createdTransaction = await _monthlyIncomeAfterTaxesService.AddMonthlyIncomeAfterTaxAsync(monthlyIncome, userId);
                return Ok(createdTransaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] MonthlyIncomeAfterTaxDto monthlyIncomeAfterTaxDto)
        {
            if (monthlyIncomeAfterTaxDto == null || id != monthlyIncomeAfterTaxDto.Id)
            {
                return BadRequest("Transaction data is invalid or ID does not match.");
            }

            try
            {
                // TODO: do I want to retrieve user ID from autheticated user context?
                // probs this kind of thing used if using 3rd party provider
                var requestingUserId = Guid.Parse(User.FindFirst("userId")?.Value ?? string.Empty);

                var updatedMonthlyIncome = await _monthlyIncomeAfterTaxesService.UpdateMonthlyIncomeAfterTaxAsync(monthlyIncomeAfterTaxDto, requestingUserId);

                if (updatedMonthlyIncome == null)
                {
                    return NotFound("Transaction not found.");
                }

                return Ok(updatedMonthlyIncome);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not authorized to update this transaction.");
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (MissingOrWrongDataException)
            {
                return BadRequest("Missing data");
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
