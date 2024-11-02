using Common.Model.Dtos;
using Common.Services.Interfaces;
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
                // Service retrieves user details using userId and verifies access
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
