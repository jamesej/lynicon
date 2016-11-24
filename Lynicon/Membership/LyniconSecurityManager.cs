using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using Lynicon.Utility;
using System.Threading;
using System.Security.Claims;
using Lynicon.Collation;

namespace Lynicon.Membership
{
    /// <summary>
    /// Security Manager provides methods to assist in forms authentication using the Lightweight Membership classes.
    /// </summary>
    public class LyniconSecurityManager : ISecurityManager
    {
        public const string UserKey = "LynCurrentUser";

        static ISecurityManager current = new LyniconSecurityManager();
        public static ISecurityManager Current { get { return current; } set { current = value; } }
        public static TSecM CurrentAs<TSecM>()
            where TSecM : class, ISecurityManager
        {
            return current as TSecM;
        }

        public static void EnsureAdminUser(string password)
        {
            LyniconSecurityManager.Current.EnsureRoles("AEU");

            var adminUser = Collator.Instance.Get<User, User>(iq => iq.Where(u => u.UserName == "administrator")).FirstOrDefault();
            if (adminUser == null)
            {
                Guid adminUserId = Guid.NewGuid();
                adminUser = Collator.Instance.GetNew<User>(new Address(typeof(User), adminUserId.ToString()));
                adminUser.Email = "admin@lynicon-user.com";
                adminUser.Id = adminUserId;
                adminUser.Roles = "AEU";
                adminUser.UserName = "administrator";
                Collator.Instance.Set(adminUser, true);
            }

            LyniconSecurityManager.Current.SetPassword(adminUser.IdAsString, password);
        }

        static LyniconSecurityManager() { }

        /// <summary>
        /// The current user (cached on request)
        /// </summary>
        public IUser User
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.User == null)
                    return null;

                IUser cached = null;
                if (GetCachedUser(out cached))
                    return cached;

                var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
                SetCachedUser(prov.GetUser(HttpContext.Current.User.Identity.Name));
                GetCachedUser(out cached);
                return cached;
            }
        }

        internal bool GetCachedUser(out IUser user)
        {
            
            if (HttpContext.Current == null || HttpContext.Current.User == null)
            {
                user = null;
                return false;
            }
                
            if (HttpContext.Current.Items.Contains(UserKey))
            {
                user = (IUser)HttpContext.Current.Items[UserKey];
                return true;
            }

            user = null;
            return false;
        }

        internal void SetCachedUser(IUser user)
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null)
                return;
            HttpContext.Current.Items[UserKey] = user;
        }

        /// <summary>
        /// Whether the current user is in a given role
        /// </summary>
        /// <param name="role">The role (a single letter)</param>
        /// <returns>True if the user is in the role</returns>
        public virtual bool CurrentUserInRole(string role)
        {
            return ((ClaimsPrincipal)Thread.CurrentPrincipal).IsInRole(role);
        }

        /// <summary>
        /// The userid of the current user
        /// </summary>
        public string UserId
        {
            get
            {
                var u = User;
                return u == null ? (string)null : u.IdAsString;
            }
        }

        /// <summary>
        /// Sets up the current user to be available via standard ASP.Net interfaces
        /// </summary>
        public void EnsureLightweightIdentity()
        {
            IPrincipal origUser = HttpContext.Current.User;

            if (origUser.Identity.IsAuthenticated && origUser.Identity.AuthenticationType == "Forms")
                SubstituteLightweightIdentity((origUser.Identity as FormsIdentity).Ticket);
        }

        /// <summary>
        /// Makes the database user record available through ASP.Net standard interfaces
        /// </summary>
        internal void SubstituteLightweightIdentity(FormsAuthenticationTicket ticket)
        {
            LightweightIdentity i = new LightweightIdentity(ticket);
            Thread.CurrentPrincipal = HttpContext.Current.User = new RolePrincipal("LightweightRoleProvider", i);
        }

        /// <summary>
        /// Encrypt a password
        /// </summary>
        /// <param name="pw">The plain text password</param>
        /// <returns>The encrypted password</returns>
        public string EncryptPassword(string pw)
        {
            var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
            return prov.EncryptPassword(pw);
        }

        /// <summary>
        /// Try to log in a user (don't persist the login)
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns>The user record or null if failed</returns>
        public IUser LoginUser(string username, string password)
        {
            return LoginUser(username, password, false);
        }
        /// <summary>
        /// Try to log in a user with an option to persist the login cookie
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="keepLoggedIn">True to persist the login</param>
        /// <returns>The user record or null if failed</returns>
        public IUser LoginUser(string username, string password, bool keepLoggedIn)
        {
            var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
            IUser user = prov.ValidateUser(username, password, null, false);
            if (user != null)
            {
                var authCookie = FormsAuthentication.GetAuthCookie(user.UserName, keepLoggedIn);
                // Set up forms auth user and custom identity values
                
                SubstituteLightweightIdentity(FormsAuthentication.Decrypt(authCookie.Value));
                HttpContext.Current.Response.Cookies.Set(authCookie);
                // Update request user cache
                HttpContext.Current.Items[UserKey] = user;

                // logged on event
                OnLogin(this, new EventArgs());
            }
            return user;
        }
        /// <summary>
        /// Log in a user without using a password (careful using this, Security Risk)
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="keepLoggedIn">Whether to persist the login cookie</param>
        /// <returns>The user's record or null if not found</returns>
        public IUser LoginUser(string username, bool keepLoggedIn)
        {
            var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
            var user = prov.GetUser(username);
            if (user != null)
            {
                var authCookie = FormsAuthentication.GetAuthCookie(user.UserName, keepLoggedIn);
                // Set up forms auth user and custom identity values

                SubstituteLightweightIdentity(FormsAuthentication.Decrypt(authCookie.Value));
                HttpContext.Current.Response.Cookies.Set(authCookie);
                // Update request user cache
                HttpContext.Current.Items[UserKey] = user;

                // logged on event
                OnLogin(this, new EventArgs());
            }
            return user;
        }

        /// <summary>
        /// Log out the current user
        /// </summary>
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Set the password for the current user (beware, security risk if misused)
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="newPw">The new password</param>
        /// <returns>True if succeeded</returns>
        public bool SetPassword(string userId, string newPw)
        {
            try
            {
                var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
                var user = Collator.Instance.Get<Lynicon.Membership.User>(new ItemId(typeof(User), userId));
                user.Password = prov.EncryptPassword(newPw);
                Collator.Instance.Set(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Event which is fired on login
        /// </summary>
        public event EventHandler<EventArgs> Login;

        internal void OnLogin(object sender, EventArgs e)
        {
            if (Login != null)
                Login(sender, e);
        }

        public void EnsureRoles(string roles)
        {
            // no action required as there is no table of roles
        }
    }
}
