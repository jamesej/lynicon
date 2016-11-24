using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace Lynicon.Tools
{
    public class ProjectContextLoader
    {
        public static EnvDTE80.DTE2 DTE2 { get; set; }
        private static string DllPath { get; set; }

        public static EnvDTE.Project MainProject { get; set; }

        public static string AssName { get; set; }

        public static string AssPath { get; set; }

        public static string DefaultNs { get; set; }

        public static string RootPath { get; set; }

        public static string WebConfigPath { get; set; }

        public static Assembly StartupAssembly { get; set; }

        private static bool ContextLoaded { get; set; }

        private static bool DataApiInitialised { get; set; }

        static ProjectContextLoader()
        {
            DTE2 = null;
            ContextLoaded = false;
            DataApiInitialised = false;
        }

        public static void EnsureDTE(Cmdlet caller)
        {
            if (DTE2 != null)
                return;

            DTE2 = DTEFinder.GetDTE();
            //var mvcProj = dte.Solution.Projects.Cast<Project>().FirstOrDefault(p => p.Kind == Constants.vsProjectKind)
            string startupProjIdx = ((Array)DTE2.Solution.SolutionBuild.StartupProjects).Cast<string>().First();
            var startupProj = DTE2.Solution.Item(startupProjIdx);
            caller.WriteVerbose("Found startup project: " + startupProj.Name);

            MainProject = startupProj;
            AssName = startupProj.Properties.Item("AssemblyName").Value.ToString();
            DefaultNs = startupProj.Properties.Item("DefaultNamespace").Value.ToString();
            RootPath = startupProj.FullName.UpToLast("\\");
            AssPath = RootPath + "\\bin\\" + AssName + ".dll";
            WebConfigPath = RootPath + "\\web.config";
        }

        public static void EnsureLoaded(Cmdlet caller)
        {
            if (ContextLoaded)
                return;

            EnsureDTE(caller);

            var progress = new ProgressRecord(0, "Initializing Project Context", "Loading assembly: " + AssPath);
            caller.WriteProgress(progress);

            System.AppDomain.CurrentDomain.SetData("APPBASE", RootPath);
            System.AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "bin");

            DllPath = RootPath + "\\bin\\";
            AppDomain.CurrentDomain.AssemblyResolve += ResolveRuntimeRedirects;

            // Use the web config of the website project as config for this appdomain
            using (AppConfig.Change(WebConfigPath))
            {
                StartupAssembly = null;
                try
                {
                    StartupAssembly = Assembly.LoadFrom(AssPath);
                    var x = StartupAssembly.GetType("log4net.Config.Log4NetConfigurationSectionHandler");
                }
                catch (Exception ex)
                {
                    ToolsHelper.WriteException(caller, ex);
                    caller.ThrowTerminatingError(new ErrorRecord(ex, "NOASSEMBLY", ErrorCategory.ReadError, AssPath));
                }

                Assembly efSqlAss = null;
                try
                {
                    // Load this manually as it will generally not be referenced but is referenced by Lynicon
                    efSqlAss = Assembly.LoadFrom(RootPath + "\\bin\\EntityFramework.SqlServer.dll");
                    var x = efSqlAss.GetType("System.Data.Entity.SqlServer.SqlProviderServices");
                }
                catch { }

                progress.StatusDescription = "Assembly loaded";
                caller.WriteProgress(progress);
            }

            ContextLoaded = true;
        }

        public static void InitialiseDataApi(Cmdlet caller)
        {
            if (DataApiInitialised)
                return;

            EnsureLoaded(caller);

            using (AppConfig.Change(WebConfigPath))
            {
                string lyniconConfigName = DefaultNs + ".LyniconConfig";
                var progress = new ProgressRecord(0, "Initializing Project Context", "Trying to load type: " + lyniconConfigName);

                caller.WriteProgress(progress);
                Type lyniconConfig = StartupAssembly.GetType(lyniconConfigName);

                progress.StatusDescription = "Type loaded";
                caller.WriteProgress(progress);

                var registerModulesMethod = lyniconConfig.GetMethod("RegisterModules", BindingFlags.Static | BindingFlags.Public);
                var initialiseDataApiMethod = lyniconConfig.GetMethod("InitialiseDataApi", BindingFlags.Static | BindingFlags.Public);
                if (registerModulesMethod == null || initialiseDataApiMethod == null)
                    caller.ThrowTerminatingError(new ErrorRecord(new Exception("Cannot find 'RegisterModules' and 'InitialiseDataApi' methods on LyniconConfig"), "NOMETHODS", ErrorCategory.ReadError, lyniconConfig));

                progress.StatusDescription = "Initializing Data Api";
                caller.WriteProgress(progress);
                try
                {
                    registerModulesMethod.Invoke(null, new object[0]);
                    initialiseDataApiMethod.Invoke(null, new object[0]);
                }

                catch (Exception ex)
                {
                    ToolsHelper.WriteException(caller, ex);
                    caller.ThrowTerminatingError(new ErrorRecord(ex, "INITIALISEDATAAPIFAIL", ErrorCategory.InvalidOperation, lyniconConfig));
                }

                progress.StatusDescription = "Lynicon initialized";
                caller.WriteProgress(progress);
            }

            DataApiInitialised = true;
        }

        private static Assembly ResolveRuntimeRedirects(object sender, ResolveEventArgs args)
        {
            try
            {
                string assName = args.Name.UpTo(",");
                string path = DllPath + assName + ".dll";
                if (File.Exists(path))
                {
                    var assembly = Assembly.LoadFrom(DllPath + assName + ".dll");
                    return assembly;
                }
                else
                    return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return null;
        }

        public static FileModel GetItemFileModel(Cmdlet caller, string itemName)
        {
            EnsureDTE(caller);

            var global = FindItemByPath(MainProject.ProjectItems, itemName);
            if (global == null)
                caller.ThrowTerminatingError(new ErrorRecord(new Exception("Can't find " + itemName + " to update"), "MISSINGPROJECTFILE", ErrorCategory.ObjectNotFound, MainProject));
            var globalFileName = global.get_FileNames(1);
            if (!globalFileName.EndsWith(".cs"))
                globalFileName += ".cs";

            var fileModel = new FileModel(globalFileName);
            return fileModel;
        }

        public static ProjectItem FindItemByPath(ProjectItems topItems, string itemPath)
        {
            ProjectItem item = topItems.OfType<ProjectItem>().FirstOrDefault(pi => pi.Name == itemPath.UpTo("/"));
            if (!itemPath.Contains("/"))
                return item;
            else
                return FindItemByPath(item.ProjectItems, itemPath.After("/"));
        }
    }
}
