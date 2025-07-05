using Common.Model.Dtos;
using Common.Services.Interfaces;
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
            var createdTransaction = await _monthlyIncomeAfterTaxesService.AddMonthlyIncomeAfterTaxAsync(monthlyIncome, userId);
            return Ok(createdTransaction);

        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] MonthlyIncomeAfterTaxDto monthlyIncomeAfterTaxDto)
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
    }
}
