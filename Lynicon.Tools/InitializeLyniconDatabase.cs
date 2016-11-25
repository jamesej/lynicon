using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using EnvDTE;
using System.Reflection;
using System.IO;

namespace Lynicon.Tools
{
    // Windows PowerShell assembly.

    // Declare the class as a cmdlet and specify and 
    // appropriate verb and noun for the cmdlet name.
    [Cmdlet(VerbsData.Initialize, "LyniconDatabase")]
    public class InitializeLyniconDatabaseCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            //ProjectContextLoader.EnsureLoaded(this);

            //using (AppConfig.Change(ProjectContextLoader.WebConfigPath))
            //{
            //    dynamic pdb = Activator.CreateInstance("Lynicon", "Lynicon.Repositories.PreloadDb").Unwrap();
            //    try
            //    {
            //        pdb.EnsureCoreDb();
            //    }
            //    catch (Exception ex)
            //    {
            //        ToolsHelper.WriteException(this, ex);
            //        ThrowTerminatingError(new ErrorRecord(ex, "DATABASEFAIL", ErrorCategory.ReadError, pdb));
            //    }
            //}

            AppDomain ad = AppDomain.CreateDomain("InitializeLyniconDatabase");
            var remote = (RemoteInitializeLyniconDatabase)ad.CreateInstanceFromAndUnwrap(
                typeof(RemoteInitializeLyniconDatabase).Assembly.Location, typeof(RemoteInitializeLyniconDatabase).FullName);

            remote.Run();
            AppDomain.Unload(ad);
        }
    }
}
