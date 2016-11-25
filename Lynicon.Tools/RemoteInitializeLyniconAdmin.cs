using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tools
{
    public class RemoteInitializeLyniconAdmin : MarshalByRefObject
    {
        public void Run(string password)
        {
            ProjectContextLoader.InitialiseDataApi(null);

            using (AppConfig.Change(ProjectContextLoader.WebConfigPath))
            {
                try
                {
                    Assembly lynicon = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.ToLower() == "lynicon");
                    Type secmType = lynicon.GetType("Lynicon.Membership.LyniconSecurityManager");
                    MethodInfo ensureAdminUser = secmType.GetMethod("EnsureAdminUser", BindingFlags.Public | BindingFlags.Static);
                    ensureAdminUser.Invoke(null, new object[] { password });
                }
                catch (Exception ex)
                {
                    //ToolsHelper.WriteException(this, ex);
                    //ThrowTerminatingError(new ErrorRecord(ex, "USERACTIONSFAIL", ErrorCategory.InvalidOperation, null));
                }
            }
        }
    }
}
