using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lynicon.Config
{
    public static class ConfigHelper
    {
        /// <summary>
        /// Get the FileManagerRoot from the configuration for Lynicon
        /// </summary>
        public static string FileManagerRoot
        {
            get
            {
                LyniconSection lyniconSection = ConfigurationManager.GetSection("lynicon/basic") as LyniconSection;
                return lyniconSection.LyniconFileManagerRoot.Value;
            }
        }

        /// <summary>
        /// Get the base app folder relative path for the Lynicon views
        /// </summary>
        /// <param name="name">The name of the view file</param>
        /// <returns>Folder relative path</returns>
        public static string GetViewPath(string name)
        {
            LyniconSection lyniconSection = ConfigurationManager.GetSection("lynicon/basic") as LyniconSection;
            string baseLyniconPath = lyniconSection.LyniconAreaBaseUrl.Value;
            return "~" + baseLyniconPath + "Views/Shared/" + name;
        }
    }
}
