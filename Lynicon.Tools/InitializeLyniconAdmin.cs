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
            AppDomain ad = AppDomain.CreateDomain("InitializeLyniconAdmin");
            var remote = (RemoteInitializeLyniconAdmin)ad.CreateInstanceFromAndUnwrap(
                typeof(RemoteInitializeLyniconAdmin).Assembly.Location, typeof(RemoteInitializeLyniconAdmin).FullName);

            remote.Run(Password);
            AppDomain.Unload(ad);
        }
    }
}
