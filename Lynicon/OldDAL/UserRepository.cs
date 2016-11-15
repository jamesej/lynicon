using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Extensibility;
using Lynicon.Membership;
using QB = Lynicon.Repositories.QueryBuilder<System.Data.SqlClient.SqlConnection, System.Data.SqlClient.SqlCommand, System.Data.SqlClient.SqlParameter, System.Data.SqlClient.SqlDataReader>;

namespace Lynicon.Repositories
{
    public class UserRepository : BasicRepository, IUserRepository
    {
        public UserRepository(string tableName) : base(tableName)
        { }
        public UserRepository(string tableName, string idName)
            : base(tableName)
        {
            this.IdName = idName;
        }

        #region IUserRepository Members

        public IUser GetUser(string userName)
        {
            PropertyStore ps = this.Get(typeof(User), new Dictionary<string, object> { { "UserName = @userName", userName } }, new List<string>(), false).FirstOrDefault();
            return ps == null ? null : ps.Project<User>();
        }

        public IUser GetUser(Guid id)
        {
            return this.Get<User>(id);
        }

        public IUser GetUserByEmail(string email)
        {
            PropertyStore ps = this.Get(typeof(User), new Dictionary<string, object> { { "Email = @email", email } }, new List<string>(), false).FirstOrDefault();
            return ps == null ? null : ps.Project<User>();
        }

        public IUser GetAnyUser(string userName, string email)
        {
            var query = QueryBuilderFactory.Instance.Create();
            query.SqlConditionals.Add("(UserName = @userName OR Email = @email)");
            query.SqlParameters.Add("@userName", userName);
            query.SqlParameters.Add("@email", email);

            query.SelectModifier = "TOP 1";
            query.SqlTable = "Users";
            PropertyStore ps = query.RunSelect().FirstOrDefault();
            return ps == null ? null : ps.Project<User>();
        }

        public IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IUser> GetUsers(IEnumerable<string> userNames)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
