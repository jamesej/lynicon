using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace Lynicon.Tools
{
    public static class DTEFinder
    {
        public static EnvDTE80.DTE2 GetDTE()
        {
            EnvDTE80.DTE2 dte2;
            try
            {
                dte2 = (EnvDTE80.DTE2)Marshal.GetActiveObject("VisualStudio.DTE.12.0");
            }
            catch
            {
                try
                {
                    dte2 = (EnvDTE80.DTE2)Marshal.GetActiveObject("VisualStudio.DTE.14.0");
                }
                catch (Exception ex)
                {
                    throw new Exception("Can't find running Visual Studio instance", ex);
                }
            }
            return dte2;
        }
    }
}
