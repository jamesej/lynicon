using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Web.Security;
using Lynicon.Membership;
using System.Web.Routing;
using Lynicon.Config;
using Lynicon.Models;
using Lynicon.Utility;

namespace Lynicon.Controllers
{
    /// <summary>
    /// Controller to deal with logging in and out as a CMS user via /lynicon/login
    /// </summary>
    public class LoginController : Controller
    {
        LightweightMembershipProvider membership = null;

        protected override void Initialize(RequestContext requestContext)
        {
            if (membership == null) { membership = System.Web.Security.Membership.Provider as LightweightMembershipProvider; }

            base.Initialize(requestContext);
        }

        /// <summary>
        /// Get the standard login form
        /// </summary>
        /// <returns>Markup for standard login form</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View(ConfigHelper.GetViewPath("LyniconLogin.cshtml"), new LoginForm());
        }

        /// <summary>
        /// Process results of login attempt
        /// </summary>
        /// <param name="login">The login form values</param>
        /// <param name="returnUrl">The url to return to</param>
        /// <returns>The markup for the login result, or redirect to return url or home page</returns>
        [HttpPost]
        public ActionResult Index(LoginForm login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                {
                    ModelState.AddModelError("", "Please supply both a user name and password");
                }
                else
                {
                    IUser user = LyniconSecurityManager.Current.LoginUser(login.UserName, login.Password, false);
                    if (user != null)
                    {
                        //membership.ChangePassword("admin", "init", "adminlocal");
                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(ConfigHelper.GetViewPath("LyniconLogin.cshtml"), login);
        }

        /// <summary>
        /// Perform a log out of the current user
        /// </summary>
        /// <param name="returnUrl">Url to return to after logging out</param>
        /// <returns>Redirect to return Url or else home page</returns>
        [Authorize(Roles = Membership.User.UserRole)]
        public ActionResult Logout(string returnUrl)
        {
            LyniconSecurityManager.Current.Logout();

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [Authorize(Roles = Membership.User.AdminRole)]
        public ActionResult ChangePassword(string userId, string newPw)
        {
            var result = LyniconSecurityManager.Current.SetPassword(userId, newPw);
            if (result)
                return Content("OK");
            else
                return Content("There was an error");
        }
    }
}
