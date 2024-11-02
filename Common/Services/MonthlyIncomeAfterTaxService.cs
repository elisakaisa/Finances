using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;

namespace Common.Services
{
    public class MonthlyIncomeAfterTaxService : IMonthlyIncomeAfterTaxesService
    {
        private readonly IUserRepository _userRepository;
        public MonthlyIncomeAfterTaxService(IUserRepository userRepo)
        {
            _userRepository = userRepo;
        }
        public Task<MonthlyIncomeAfterTax> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateThatUserIsInHousehold(requestingUserId, monthlyIncomeAfterTax.User.HouseholdId);
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateThatUserIsInHousehold(requestingUserId, monthlyIncomeAfterTax.User.HouseholdId);
            throw new NotImplementedException();
        }

        private async void ValidateThatUserIsInHousehold(Guid requestingUserId, Guid householdId)
        {
            var requestingUser = await _userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }

        }
    }
}
