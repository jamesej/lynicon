using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Lynicon.Utility;
using Lynicon.Membership;
using Lynicon.Collation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using EnvDTE;
using System.Reflection;
using System.IO;
using Lynicon.Repositories;

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
            ProjectContextLoader.EnsureLoaded(this);

            using (AppConfig.Change(ProjectContextLoader.WebConfigPath))
            {
                var pdb = new PreloadDb();
                pdb.EnsureCoreDb();
            }
        }
    }
}
