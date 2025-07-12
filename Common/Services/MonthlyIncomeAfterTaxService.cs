using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class MonthlyIncomeAfterTaxService(IUserRepository userRepository, IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepository) : IMonthlyIncomeAfterTaxesService
    {
        public async Task<MonthlyIncomeAfterTaxDto> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateMonthlyIncomeData(monthlyIncomeAfterTax);

            var user = await userRepository.GetByIdAsync(monthlyIncomeAfterTax.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);

            var newMonthlyIncome = monthlyIncomeAfterTax.ConvertToDbObject(user);
            var newMonthlyIncome2 = await monthlyIncomeAfterTaxRepository.CreateAsync(newMonthlyIncome);
            return newMonthlyIncome2.ConvertToDto();
        }

        public async Task<MonthlyIncomeAfterTaxDto> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId)
        {
            ValidateMonthlyIncomeData(monthlyIncomeAfterTax);

            var user = await userRepository.GetByIdAsync(monthlyIncomeAfterTax.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);

            var updatedMonthlyIncome = monthlyIncomeAfterTax.ConvertToDbObject(user);

            var updatedMonthlyIncome2 = await monthlyIncomeAfterTaxRepository.UpdateAsync(updatedMonthlyIncome);
            return updatedMonthlyIncome2.ConvertToDto();
        }

        private static void ValidateMonthlyIncomeData(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTaxDto)
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
            var requestingUser = await userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }
        }
    }
}
