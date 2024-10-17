using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class RepartitionService : IRepartitionService
    {
        private ITransactionRepository _transactionRepository;
        private IFinancialMonthRepository _financialMonthRepository;
        private IMonthlyIncomeAfterTaxRepository _monthlyIncomeAfterTaxRepository;
        public RepartitionService(ITransactionRepository transactionRepo, IFinancialMonthRepository financialMonthRepo, IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepo) 
        { 
            _transactionRepository = transactionRepo;
            _financialMonthRepository = financialMonthRepo;
            _monthlyIncomeAfterTaxRepository = monthlyIncomeAfterTaxRepo;
        }

        public Task<Repartition> GetMonthlyHouseholdRepartition(Guid householdId, string monthYear)
        {
            //TODO: split into household has single user, household has 2 users, and household has more than 2 users
            throw new NotImplementedException();
        }
    }
}
