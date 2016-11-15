using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.Tools
{
    public class ProjectContextLoader
    {
        public static EnvDTE80.DTE2 DTE2 { get; set; }
        private static string DllPath { get; set; }

        public static string WebConfigPath { get; set; }

        static ProjectContextLoader()
        {
            DTE2 = null;
        }

        public static void Ensure(Cmdlet caller)
        {
            if (DTE2 != null)
                return;

            DTE2 = DTEFinder.GetDTE();
            //var mvcProj = dte.Solution.Projects.Cast<Project>().FirstOrDefault(p => p.Kind == Constants.vsProjectKind)
            string startupProjIdx = ((Array)DTE2.Solution.SolutionBuild.StartupProjects).Cast<string>().First();
            var startupProj = DTE2.Solution.Item(startupProjIdx);
            caller.WriteVerbose("Found startup project: " + startupProj.Name);

            var website = startupProj as VsWebSite.VSWebSite;
            string assName = startupProj.Properties.Item("AssemblyName").Value.ToString();
            string defaultNs = startupProj.Properties.Item("DefaultNamespace").Value.ToString();
            string rootPath = startupProj.FullName.UpToLast("\\");
            string assPath = rootPath + "\\bin\\" + assName + ".dll";
            WebConfigPath = rootPath + "\\web.config";
            caller.WriteVerbose("Trying to load assembly at: " + assPath);

            System.AppDomain.CurrentDomain.SetData("APPBASE", rootPath);
            System.AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "bin");

            DllPath = rootPath + "\\bin\\";
            AppDomain.CurrentDomain.AssemblyResolve += ResolveRuntimeRedirects;

            // Use the web config of the website project as config for this appdomain
            using (AppConfig.Change(WebConfigPath))
            {
                Assembly startupAss = null;
                try
                {
                    startupAss = Assembly.LoadFrom(assPath);
                    var x = startupAss.GetType("log4net.Config.Log4NetConfigurationSectionHandler");
                }
                catch (Exception ex)
                {
                    caller.ThrowTerminatingError(new ErrorRecord(ex, "NOASSEMBLY", ErrorCategory.ReadError, assPath));
                }

                Assembly efSqlAss = null;
                try
                {
                    // Load this manually as it will generally not be referenced but is referenced by Lynicon
                    efSqlAss = Assembly.LoadFrom(rootPath + "\\bin\\EntityFramework.SqlServer.dll");
                    var x = efSqlAss.GetType("System.Data.Entity.SqlServer.SqlProviderServices");
                }
                catch { }


                string lyniconConfigName = defaultNs + ".LyniconConfig";
                caller.WriteVerbose("Trying to load type: " + lyniconConfigName);
                Type lyniconConfig = startupAss.GetType(lyniconConfigName);

                var registerModulesMethod = lyniconConfig.GetMethod("RegisterModules", BindingFlags.Static | BindingFlags.Public);
                var initialiseDataApiMethod = lyniconConfig.GetMethod("InitialiseDataApi", BindingFlags.Static | BindingFlags.Public);
                if (registerModulesMethod == null || initialiseDataApiMethod == null)
                    caller.ThrowTerminatingError(new ErrorRecord(new Exception("Cannot find 'RegisterModules' and 'InitialiseDataApi' methods on LyniconConfig"), "NOMETHODS", ErrorCategory.ReadError, lyniconConfig));

                try
                {
                    registerModulesMethod.Invoke(null, new object[0]);
                    initialiseDataApiMethod.Invoke(null, new object[0]);
                }

                catch (Exception ex)
                {
                    caller.WriteVerbose(string.Format("Exception was: {0} at: {1}", ex.Message, ex.StackTrace));
                    caller.ThrowTerminatingError(new ErrorRecord(ex, "INITIALISEDATAAPIFAIL", ErrorCategory.InvalidOperation, lyniconConfig));
                }
            }
        }

        private static Assembly ResolveRuntimeRedirects(object sender, ResolveEventArgs args)
        {
            // Check that CLR is loading the version of ThirdPartyLibrary referenced by MyLibrary
            if (args.Name.StartsWith("Microsoft.Owin") || args.Name.StartsWith("log4net"))
            {
                try
                {
                    string assName = args.Name.UpTo(",");
                    var assembly = Assembly.LoadFrom(DllPath + assName + ".dll");
                    return assembly;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            return null;
        }
    }
}
