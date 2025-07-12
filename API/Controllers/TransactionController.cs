using Common.Model.Dtos;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transaction, [FromHeader] Guid userId)
        {
            var createdTransaction = await transactionService.CreateAsync(transaction, userId);
            return Ok(createdTransaction);
        }


        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionDto transactionDto)
        {
            // TODO: do I want to retrieve user ID from autheticated user context?
            // probs this kind of thing used if using 3rd party provider
            var requestingUserId = Guid.Parse(User.FindFirst("userId")?.Value ?? string.Empty);

            var updatedTransaction = await transactionService.UpdateAsync(transactionDto, requestingUserId);

            if (updatedTransaction == null)
            {
                return NotFound("Transaction not found.");
            }

            return Ok(updatedTransaction);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteTransaction([FromBody] TransactionDto transactionDto, [FromQuery] Guid requestingUserId)
        {
            bool result = await transactionService.DeleteAsync(transactionDto, requestingUserId);

            if (result)
            {
                return Ok(new { success = true, message = "Transaction deleted successfully." });
            }
            else
            {
                return NotFound(new { success = false, message = "Transaction not found or could not be deleted." });
            }
        }


        [HttpGet("household/monthly-transactions")]
        public async Task<IActionResult> GetMonthlyTransactionsByHousehold([FromQuery] string financialMonth, [FromHeader] Guid requestingUserId)
        {
            var transactions = await transactionService.GetMonthlyTransactionsByHousehold(financialMonth, requestingUserId);
            return Ok(transactions);
        }


        [HttpGet("household/yearly-transactions")]
        public async Task<IActionResult> GetYearlyTransactionsByHousehold([FromQuery] int year, [FromHeader] Guid requestingUserId)
        {
            var transactions = await transactionService.GetYearlyTransactionsByHousehold(year, requestingUserId);
            return Ok(transactions);
        }
    }
}
