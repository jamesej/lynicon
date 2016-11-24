using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Linq.Expressions;

namespace Lynicon.Tools
{
    // Windows PowerShell assembly.

    // Declare the class as a cmdlet and specify and 
    // appropriate verb and noun for the cmdlet name.
    [Cmdlet(VerbsData.Initialize, "LyniconAdmin")]
    public class InitializeLyniconAdminCommand : Cmdlet
    {
        // Declare the parameters for the cmdlet.
        [Parameter(Mandatory = true)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private string password;

        private object GetStaticPropVal(Type type, string propName)
        {
            var pi = type.GetProperty("Instance", BindingFlags.GetProperty | BindingFlags.Static);
            return pi.GetValue(null);
        }

        protected override void ProcessRecord()
        {
            ProjectContextLoader.InitialiseDataApi(this);

            using (AppConfig.Change(ProjectContextLoader.WebConfigPath))
            {
                try
                {
                    // construct 'var adminUser = Collator.Instance.Get<User, User>(iq => iq.Where(u => u.UserName == "administrator").FirstOrDefault()

                    //object coll = Activator.CreateInstance("Lynicon", "Lynicon.Collation.Collator").Unwrap();
                    //object collInst = GetStaticPropVal(coll.GetType(), "Instance");
                    //MethodInfo getMethod = collInst.GetType().GetMethods()
                    //    .First(mi => mi.IsGenericMethod
                    //                 && mi.GetGenericArguments().Length == 2
                    //                 && mi.GetParameters().Length == 1);
                    
                    //Type tUser = coll.GetType().Assembly.GetType("Lynicon.Membership.User");
                    //PropertyInfo userName = tUser.GetProperty("UserName");
                    //var uParam = Expression.Parameter(tUser, "u");
                    //var accessUserName = Expression.MakeMemberAccess(uParam, userName);
                    //var cmpUserName = Expression.Equal(accessUserName, Expression.Constant("administrator"));
                    //var lmdWhere = Expression.Lambda(cmpUserName, uParam);
                    //MethodInfo getUserMethod = getMethod.MakeGenericMethod(tUser, tUser);
                    //var getAllUsers = getUserMethod.Invoke(collInst, new object[] { lmdWhere });
                    //var getFirstUser = typeof(Enumerable)
                    //    .GetMethod("FirstOrDefault", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null);
                    //dynamic adminUser = getFirstUser.Invoke(null, new object[] { getAllUsers });

                    Assembly lynicon = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.ToLower() == "lynicon");
                    Type secmType = lynicon.GetType("Lynicon.Membership.LyniconSecurityManager");
                    MethodInfo ensureAdminUser = secmType.GetMethod("EnsureAdminUser", BindingFlags.Public | BindingFlags.Static);
                    ensureAdminUser.Invoke(null, new object[] { Password });

                    //var adminUser = Collator.Instance.Get<User, User>(iq => iq.Where(u => u.UserName == "administrator")).FirstOrDefault();
                    //if (adminUser == null)
                    //{
                    //    Guid adminUserId = Guid.NewGuid();
                    //    adminUser = Collator.Instance.GetNew<User>(new Address(typeof(User), adminUserId.ToString()));
                    //    adminUser.Email = "admin@lynicon-user.com";
                    //    adminUser.Id = adminUserId;
                    //    adminUser.Roles = "AEU";
                    //    adminUser.UserName = "administrator";
                    //    Collator.Instance.Set(adminUser, true);
                    //}

                    //LyniconSecurityManager.Current.SetPassword(adminUser.IdAsString, Password);
                }
                catch (Exception ex)
                {
                    ToolsHelper.WriteException(this, ex);
                    ThrowTerminatingError(new ErrorRecord(ex, "USERACTIONSFAIL", ErrorCategory.InvalidOperation, null));
                }
            }
        }
    }
}
