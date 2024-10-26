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
        protected void ValidateThatUserIsInHousehold(User user, Guid householdId)
        {
            if (user.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }
        }
    }
}
