using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Membership;

namespace Lynicon.Repositories
{
    /// <summary>
    /// Interface for how a User Repository extend a Repository
    /// </summary>
    public interface IUserRepository: IRepository
    {
        /// <summary>
        /// Get the user by username
        /// </summary>
        /// <param name="userName">The user's username</param>
        /// <returns>The user</returns>
        IUser GetUser(string userName);

        /// <summary>
        /// Get the user by id
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <returnsThe user></returns>
        IUser GetUser(Guid id);

        /// <summary>
        /// Get the user by email
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>The user</returns>
        IUser GetUserByEmail(string email);

        /// <summary>
        /// Get a user by either email or username
        /// </summary>
        /// <param name="userName">The user name if supplied</param>
        /// <param name="email">The email if supplied</param>
        /// <returns>The user</returns>
        IUser GetAnyUser(string userName, string email);

        /// <summary>
        /// Get a list of users in different ways
        /// </summary>
        /// <param name="userName">If has a value, get the user by username</param>
        /// <param name="email">If has a value, get the user by email</param>
        /// <param name="pageIndex">Get a list of users at a page index</param>
        /// <param name="pageSize">Get a list of users of a page size</param>
        /// <param name="totalRecords">Outputs the total count of records in the list</param>
        /// <returns>The list of users</returns>
        IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords);
        /// <summary>
        /// Get a list of users by a list of username
        /// </summary>
        /// <param name="userNames">list of user names</param>
        /// <returns>list of users</returns>
        IEnumerable<IUser> GetUsers(IEnumerable<string> userNames);
    }
}
