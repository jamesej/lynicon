using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

using System.Configuration;
using System.Web.Configuration;
using System.Configuration.Provider;
using Lynicon.Repositories;
using Lynicon.Membership;

namespace Lynicon.Membership
{
    /// <summary>
    /// Role provider for Lynicon's custom ASP.Net membership provider.  Roles are single characters
    /// to simplify storage
    /// </summary>
    public class LightweightRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.Any(rn => rn.Length != 1))
                throw new ProviderException("Role names are single characters only");

            try
            {
                var repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                List<IUser> users = repo.GetUsers(usernames).ToList();
                foreach (IUser user in users)
                    user.Roles = new string((user.Roles + string.Join("", roleNames)).ToCharArray().Distinct().ToArray());
                repo.Set(users.Cast<object>().ToList(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to add users to roles", ex);
            }
        }

        private string applicationName = null;
        public override string ApplicationName
        {
            get
            {
                if (applicationName == null && HttpContext.Current != null && HttpContext.Current.Request != null)
                    return HttpContext.Current.Request.ApplicationPath;
                else
                    return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Split(null);
        }

        protected virtual IUser GetUserByName(string username)
        {
            IUser cached = null;
            LyniconSecurityManager.CurrentAs<LyniconSecurityManager>().GetCachedUser(out cached);
            IUser user = null;
            if (cached == null || cached.UserName != username)
            {
                var repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                user = repo.GetUser(username);
                LyniconSecurityManager.CurrentAs<LyniconSecurityManager>().SetCachedUser(user);
            }
            else
                user = cached;

            return user;
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                var user = GetUserByName(username);

                if (user == null)
                    return new string[0];
                else
                {
                    var roles = user.Roles.ToCharArray().Select(c => c.ToString()).ToArray();
                    return roles;
                }
                    
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get roles for user", ex);
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            try
            {
                if (roleName.Length != 1)
                    throw new ProviderException("Role names are single characters only");
                var repo = Repository.Instance.Registered(typeof(User)) as UserRepository;
                var xs = repo
                    .Get<User>(typeof(User), new Type[] { typeof(User) }, iq => iq.Where(u => u.Roles.Contains(roleName)))
                    .Select(u => u.UserName).ToArray();
                return xs;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get users in role", ex);
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                if (roleName.Length != 1)
                    throw new ProviderException("Role names are single characters only");

                var user = GetUserByName(username);

                if (user == null)
                    return false;
                return user.Roles.Contains(roleName);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get users in role", ex);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.Any(rn => rn.Length != 1))
                throw new ProviderException("Role names are single characters only");

            try
            {
                var repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                var users = repo.GetUsers(usernames);
                char[] roleChars = string.Join("", roleNames).ToCharArray();
                foreach (IUser user in users)
                    user.Roles = new string(user.Roles.ToCharArray().Except(roleChars).ToArray());
                repo.Set(users.Cast<object>().ToList(), new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to remove users from roles", ex);
            }
        }

        public override bool RoleExists(string roleName)
        {
            return GetAllRoles().Contains(roleName);
        }
    }
}
