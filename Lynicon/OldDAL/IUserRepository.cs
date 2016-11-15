using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Membership;

namespace Lynicon.Repositories
{
    public interface IUserRepository: IRepository
    {
        IUser GetUser(string userName);
        IUser GetUser(Guid id);
        IUser GetUserByEmail(string email);
        IUser GetAnyUser(string userName, string email);
        IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords);
        IEnumerable<IUser> GetUsers(IEnumerable<string> userNames);
    }
}
