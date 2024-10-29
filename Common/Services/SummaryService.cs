using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class SummaryService : BaseService, ISummaryService
    {
        private readonly ITransactionRepository _transactionRepository;

        public SummaryService(ITransactionRepository transactionRepo)
        {
            _transactionRepository = transactionRepo;
        }

        public Task<List<MonthlyHouseholdTransactionSum>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            throw new NotImplementedException();
        }

        public Task<List<MonthlyHouseholdTransactionSum>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            throw new NotImplementedException();
        }
    }
}
