using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.DataSources;

namespace Lynicon.Repositories
{
    public class UserRepository : BasicRepository, IUserRepository
    {
        public UserRepository() : base(new CoreDataSourceFactory())
        { }
        public UserRepository(IDataSourceFactory dataSourceFactory) : base(dataSourceFactory)
        { }
        public UserRepository(string idName)
            : this()
        {
            this.IdName = idName;
        }

        #region IUserRepository Members

        /// <summary>
        /// Get the user by username
        /// </summary>
        /// <param name="userName">The user's username</param>
        /// <returns>The user</returns>
        public IUser GetUser(string userName)
        {
            return this.Get<User>(typeof(User), new Type[] { typeof(User) }, iq => iq.Where(u => u.UserName == userName)).FirstOrDefault();
        }


        /// <summary>
        /// Get the user by id
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <returnsThe user></returns>
        public IUser GetUser(Guid id)
        {
            return this.Get<User>(typeof(User), new ItemId[] { new ItemId(typeof(User), id) }).FirstOrDefault();
        }

        /// <summary>
        /// Get the user by email
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>The user</returns>
        public IUser GetUserByEmail(string email)
        {
            return this.Get<User>(typeof(User), new Type[] { typeof(User) }, iq => iq.Where(u => u.Email == email)).FirstOrDefault();
        }

        /// <summary>
        /// Get a user by either email or username
        /// </summary>
        /// <param name="userName">The user name if supplied</param>
        /// <param name="email">The email if supplied</param>
        /// <returns>The user</returns>
        public IUser GetAnyUser(string userName, string email)
        {
            return this.Get<User>(typeof(User), new Type[] { typeof(User) }, iq => iq.Where(u => u.UserName == userName || u.Email == email)).FirstOrDefault();
        }

        /// <summary>
        /// Get a list of users in different ways
        /// </summary>
        /// <param name="userName">If has a value, get the user by username</param>
        /// <param name="email">If has a value, get the user by email</param>
        /// <param name="pageIndex">Get a list of users at a page index</param>
        /// <param name="pageSize">Get a list of users of a page size</param>
        /// <param name="totalRecords">Outputs the total count of records in the list</param>
        /// <returns>The list of users</returns>
        public IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a list of users by a list of username
        /// </summary>
        /// <param name="userNames">list of user names</param>
        /// <returns>list of users</returns>
        public IEnumerable<IUser> GetUsers(IEnumerable<string> userNames)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
