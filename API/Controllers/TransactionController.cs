using Common.Model.Dtos;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transaction, [FromHeader] Guid userId)
        {
            if (transaction == null || userId == Guid.Empty)
            {
                return BadRequest("Transaction data or user ID is missing.");
            }

            try
            {
                var createdTransaction = await _transactionService.CreateAsync(transaction, userId);
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
            catch (FinancialMonthOfWrongFormatException)
            {
                return BadRequest("Financial month of wrong format exception");
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


        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null || id != transactionDto.Id)
            {
                return BadRequest("Transaction data is invalid or ID does not match.");
            }

            try
            {
                // TODO: do I want to retrieve user ID from autheticated user context?
                // probs this kind of thing used if using 3rd party provider
                var requestingUserId = Guid.Parse(User.FindFirst("userId")?.Value ?? string.Empty);

                var updatedTransaction = await _transactionService.UpdateAsync(transactionDto, requestingUserId);

                if (updatedTransaction == null)
                {
                    return NotFound("Transaction not found.");
                }

                return Ok(updatedTransaction);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not authorized to update this transaction.");
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (FinancialMonthOfWrongFormatException)
            {
                return BadRequest("Financial month of wrong format exception");
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

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteTransaction([FromBody] TransactionDto transactionDto, [FromQuery] Guid requestingUserId)
        {
            if (transactionDto == null || requestingUserId == Guid.Empty)
            {
                return BadRequest("Transaction data is required.");
            }

            try
            {
                bool result = await _transactionService.DeleteAsync(transactionDto, requestingUserId);

                if (result)
                {
                    return Ok(new { success = true, message = "Transaction deleted successfully." });
                }
                else
                {
                    return NotFound(new { success = false, message = "Transaction not found or could not be deleted." });
                }
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("User is not authorized to delete this transaction.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }


        [HttpGet("household/{householdId}/monthly-transactions")]
        public async Task<IActionResult> GetMonthlyTransactionsByHouseholdId([FromRoute] Guid householdId, [FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || financialMonth == null || !financialMonth.IsFinancialMonthOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, financial month, and requesting user ID are required.");
            }

            try
            {
                var transactions = await _transactionService.GetMonthlyTransactionsByHouseholdId(householdId, financialMonth, requestingUserId);
                return Ok(transactions);
            }
            catch (UserNotInHouseholdException)
            {
                return Forbid("User is not authorized to view transactions for this household.");
            }
            catch (FinancialMonthOfWrongFormatException)
            {
                return BadRequest("Financial month of wrong format exception");
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }


        [HttpGet("household/{householdId}/yearly-transactions")]
        public async Task<IActionResult> GetYearlyTransactionsByHouseholdId([FromRoute] Guid householdId, [FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || !year.IsYearOfCorrectFormat() || requestingUserId == Guid.Empty)
            {
                return BadRequest("Household ID, valid year, and requesting user ID are required.");
            }

            try
            {
                var transactions = await _transactionService.GetYearlyTransactionsByHouseholdId(householdId, year, requestingUserId);
                return Ok(transactions);
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
    }
}
