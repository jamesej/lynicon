using Lynicon.Membership;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web;
using Lynicon.Collation;
using Lynicon.Repositories;
using Lynicon.Models;
using Lynicon.DataSources;

namespace Lynicon.AspNet.Identity
{
    /// <summary>
    /// An ISecurityManager to work with ASP.Net Identity
    /// </summary>
    /// <typeparam name="TContext">The client user database context type (e.g. ApplicationDbContext)</typeparam>
    /// <typeparam name="TUser">The client user record type (e.g. ApplicationUser)</typeparam>
    public class LyniconIdentitySecurityManager<TUser, TKey, TContext, TUserManager, TSignInManager> : ISecurityManager
        where TUser : IdentityUser, IUser<TKey>, new()
        where TContext : IdentityDbContext<TUser>, new()
        where TUserManager : UserManager<TUser>
        where TKey : IEquatable<TKey>
        where TSignInManager : SignInManager<TUser, TKey>
    {
        Func<UserManager<TUser>> userManager;
        Func<SignInManager<TUser, TKey>> signInManager;
        Func<IAuthenticationManager> authenticationManager;
        Func<IdentityDbContext<TUser>> context;

        public LyniconIdentitySecurityManager()
        {
            this.context = () => new TContext();
            this.userManager = () => HttpContext.Current.GetOwinContext().GetUserManager<TUserManager>();
            this.signInManager = () => HttpContext.Current.GetOwinContext().Get<TSignInManager>();
            this.authenticationManager = () => HttpContext.Current.GetOwinContext().Authentication;
        }

        /// <summary>
        /// Configure the data system to map requests for User type onto TUser via Identity mechanisms
        /// </summary>
        public virtual void InitialiseDataApi()
        {
            ContentTypeHierarchy.RegisterType(typeof(TUser));
            CompositeTypeManager.Instance.RegisterExtensionType(typeof(LyniconIdentityUser));

            var efDSFactory = new EFDataSourceFactory<TContext>();
            var appDbRepository = new BasicRepository(efDSFactory);
            efDSFactory.DbSetSelectors[typeof(TUser)] = db => db.Users.Include(u => u.Roles);
            efDSFactory.ContextLifetimeMode = ContextLifetimeMode.PerCall;

            // We DON'T want to register TUser with CompositeTypeManager
            Collator.Instance.Register(typeof(TUser), new BasicCollator(Repository.Instance));
            Repository.Instance.Register(typeof(TUser), appDbRepository);

            // override existing collator registration for User
            var identityAdaptorCollator = new IdentityAdaptorCollator<TUser, TUserManager>();
            identityAdaptorCollator.Repository = Repository.Instance;
            Collator.Instance.Register(typeof(User), identityAdaptorCollator);
        }

        /// <summary>
        /// The current user (cached on request)
        /// </summary>
        public Lynicon.Membership.IUser User
        {
            get
            {
                var um = userManager();
                if (this.UserId == null)
                    return null;
                TUser user = um.FindById(this.UserId);
                var lynUser = new LyniconIdentityUser
                {
                    IdAsString = user.Id,
                    UserName = user.UserName,
                    Email = um.GetEmail(user.Id),
                    Roles = new string(
                           um.GetRoles(user.Id)
                           .Where(r => r.Length == 1)
                           .Select(r => r[0])
                           .ToArray())
                };
                return lynUser;
            }
        }

        /// <summary>
        /// The userid of the current user
        /// </summary>
        public string UserId
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
        }

        /// <summary>
        /// Whether the current user is in a given role
        /// </summary>
        /// <param name="role">The role (a single letter)</param>
        /// <returns>True if the user is in the role</returns>
        public bool CurrentUserInRole(string role)
        {
            string userId = UserId;
            if (userId == null)
                return false;
            return userManager().IsInRole(UserId, role);
        }

        /// <summary>
        /// Try to log in a user with an option to persist the login cookie
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <param name="keepLoggedIn">True to persist the login</param>
        /// <returns>The user record or null if failed</returns>
        public Lynicon.Membership.IUser LoginUser(string username, string password, bool keepLoggedIn)
        {
            var result = signInManager().PasswordSignIn(username, password, keepLoggedIn, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Collator.Instance.Get<User, User>(iq => iq.Where(u => u.UserName == username)).FirstOrDefault();
                case SignInStatus.LockedOut:
                    return null;
                case SignInStatus.RequiresVerification:
                    return null;
                case SignInStatus.Failure:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Log out the current user
        /// </summary>
        public void Logout()
        {
            authenticationManager().SignOut();
        }

        /// <summary>
        /// Set the password for the current user (beware, security risk if misused)
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="newPw">The new password</param>
        /// <returns>True if succeeded</returns>
        public bool SetPassword(string userId, string newPw)
        {
            return Task.Run(() => SetPasswordAsync(userId, newPw)).Result;
        }

        private async Task<bool> SetPasswordAsync(string userId, string newPw)
        {
            try
            {
                UserStore<TUser> store = new UserStore<TUser>(this.context());
                UserManager<TUser> UserManager = new UserManager<TUser>(store);
                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPw);
                TUser cUser = await store.FindByIdAsync(userId);
                await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                await store.UpdateAsync(cUser);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}