using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;

namespace Common.Services
{
    public class RepartitionService : IRepartitionService
    {
        private ITransactionRepository _transactionRepository;
        private IFinancialMonthRepository _financialMonthRepository;
        private IMonthlyIncomeAfterTaxRepository _monthlyIncomeAfterTaxRepository;
        private IHouseholdRepository _householdRepository;
        public RepartitionService(
            ITransactionRepository transactionRepo, IFinancialMonthRepository financialMonthRepo,
            IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepo, IHouseholdRepository householdRepo) 
        { 
            _transactionRepository = transactionRepo;
            _financialMonthRepository = financialMonthRepo;
            _monthlyIncomeAfterTaxRepository = monthlyIncomeAfterTaxRepo;
            _householdRepository = householdRepo;
        }

        public async Task<Repartition> GetMonthlyHouseholdRepartition(Guid householdId, string monthYear)
        {
            var household = await _householdRepository.GetByIdAsync(householdId);
            if (household.Users.Count > 2) 
            {
                throw new HouseholdWithMoreThanTwoUsersNotSupportedException();
            }
            throw new NotImplementedException();
        }
    }
}
