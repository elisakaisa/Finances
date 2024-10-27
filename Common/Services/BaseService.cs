using Common.Model.DatabaseObjects;
using Common.Utils.Exceptions;

namespace Common.Services
{
    public class BaseService
    {
        public BaseService() { }  
        
        /// <summary>
        /// Ensures that users can't access other household's transactions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="householdId"></param>
        /// <exception cref="UserNotInHouseholdException"></exception>
        protected static void ValidateThatUserIsInHousehold(User user, Guid householdId)
        {
            if (user.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }
        }

        /// <summary>
        /// Ensures that users cannot create transactions for users in other households
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="user"></param>
        /// <exception cref="UserNotInHouseholdException"></exception>
        protected static void ValidateThatUserIsInHousehold(Transaction transaction, User user)
        {
            if (transaction.User.HouseholdId != user.HouseholdId)
            {
                throw new UserNotInHouseholdException();
            }
        }
    }
}
