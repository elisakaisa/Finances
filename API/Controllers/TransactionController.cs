using Common.Model.Dtos;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpPost(Name = "CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transaction, [FromHeader] Guid userId)
        {
            if (transaction == null || userId == Guid.Empty)
            {
                return BadRequest("Transaction data or user ID is missing.");
            }

            try
            {
                var createdTransaction = await _transactionService.CreateAsync(transaction, userId);
                return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the transaction.");
            }
        }


        [HttpGet("household/{householdId}/monthly-transactions")]
        public async Task<IActionResult> GetMonthlyTransactionsByHouseholdId([FromRoute] Guid householdId, [FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            if (householdId == Guid.Empty || financialMonth == null || financialMonth.IsFinancialMonthOfCorrectFormat() || requestingUserId == Guid.Empty)
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
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }

        // TODO: fix this
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            //var transaction = await _transactionService.GetByIdAsync(id);
            //Transaction transaction = null;
            //if (transaction == null)
            //{
            //    return NotFound();
            //}

            //return Ok(transaction);
            return Ok();
        }
    }
}
