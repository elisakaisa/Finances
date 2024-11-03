using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class MonthlyIncomeAfterTaxService : IMonthlyIncomeAfterTaxesService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMonthlyIncomeAfterTaxRepository _monthlyIncomeAfterTaxRepository;
        public MonthlyIncomeAfterTaxService(IUserRepository userRepo, IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepo)
        {
            _userRepository = userRepo;
            _monthlyIncomeAfterTaxRepository = monthlyIncomeAfterTaxRepo;
        }
        public async Task<MonthlyIncomeAfterTaxDto> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateMonthlyIncomeData(monthlyIncomeAfterTax);

            var user = await _userRepository.GetByIdAsync(monthlyIncomeAfterTax.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);

            var newMonthlyIncome = monthlyIncomeAfterTax.ConvertToDbObject(user);
            var newMonthlyIncome2 = await _monthlyIncomeAfterTaxRepository.CreateAsync(newMonthlyIncome);
            return newMonthlyIncome2.ConvertToDto();
        }

        public async Task<MonthlyIncomeAfterTaxDto> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateMonthlyIncomeData(monthlyIncomeAfterTax);

            var user = await _userRepository.GetByIdAsync(monthlyIncomeAfterTax.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);

            var updatedMonthlyIncome = monthlyIncomeAfterTax.ConvertToDbObject(user);

            var updatedMonthlyIncome2 = await _monthlyIncomeAfterTaxRepository.UpdateAsync(updatedMonthlyIncome);
            return updatedMonthlyIncome2.ConvertToDto();
        }

        private void ValidateMonthlyIncomeData(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTaxDto)
        {
            if (monthlyIncomeAfterTaxDto == null || 
                monthlyIncomeAfterTaxDto.Id == Guid.Empty || 
                !monthlyIncomeAfterTaxDto.FinancialMonth.IsFinancialMonthOfCorrectFormat() ||
                monthlyIncomeAfterTaxDto.UserId == Guid.Empty)
            {
                throw new MissingOrWrongDataException();
            }
        }

        private async Task ValidateThatUserIsInHousehold(Guid requestingUserId, Guid householdId)
        {
            var requestingUser = await _userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }
        }
    }
}
