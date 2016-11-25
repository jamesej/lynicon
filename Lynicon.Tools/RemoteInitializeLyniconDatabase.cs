using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tools
{
    public class RemoteInitializeLyniconDatabase : MarshalByRefObject
    {
        public void Run()
        {
            ProjectContextLoader.EnsureLoaded(null);

            using (AppConfig.Change(ProjectContextLoader.WebConfigPath))
            {
                dynamic pdb = Activator.CreateInstance("Lynicon", "Lynicon.Repositories.PreloadDb").Unwrap();
                try
                {
                    pdb.EnsureCoreDb();
                }
                catch (Exception ex)
                {
                    //ToolsHelper.WriteException(null, ex);
                    //ThrowTerminatingError(new ErrorRecord(ex, "DATABASEFAIL", ErrorCategory.ReadError, pdb));
                }
            }
        }
    }
}
