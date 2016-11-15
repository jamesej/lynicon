using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using Lynicon.Config;
using Lynicon.Utility;
using Lynicon.Attributes;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;

namespace Lynicon.Controllers
{
    /// <summary>
    /// General webservices for managing the file manager, plus other utilities for the CMS UI
    /// </summary>
    public class AjaxController : Controller
    {
        /// <summary>
        /// Get the folders in a folder in the file manager
        /// </summary>
        /// <param name="dir">the path to the folder</param>
        /// <returns>JSON list of folder information</returns>
        [NoCache, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult FileTreeFolders(string dir)
        {
            if (string.IsNullOrEmpty(dir)) dir = "/";

            if (!dir.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");

            DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath(dir));
            var output = di.GetDirectories()
                            .OrderBy(cdi => cdi.Name)
                            .Select(cdi => new
                                {
                                    data = new { title = cdi.Name },
                                    state = "closed",
                                    attr = new { title = dir + cdi.Name + "/" }
                                }).ToArray();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get the files in a folder in the file manager
        /// </summary>
        /// <param name="dir">the path to the folder</param>
        /// <returns>JSON list of file information</returns>
        [NoCache, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult FileTreeFiles(string dir)
        {
            if (string.IsNullOrEmpty(dir)) dir = "/";

            if (!dir.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");

            DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath(dir));
            var output = new
            {
                dir = !di.Exists ? null : dir + (dir[dir.Length - 1] == '/' ? "" : "/"),
                dirs = !di.Exists ? null : di.GetDirectories()
                    .OrderBy(cdi => cdi.Name)
                    .Select(cdi => new
                        {
                            name = cdi.Name
                        }).ToArray(),
                files = !di.Exists ? null : di.GetFiles()
                    .OrderBy(cfi => cfi.Name)
                    .Select(cfi => new
                        {
                            name = cfi.Name,
                            ext = cfi.Extension == null || cfi.Extension.Length < 2 ? "" : cfi.Extension.Substring(1),
                            size = cfi.Length
                        }).OrderBy(inf => inf.name).ToArray()
            };
            return Json(output, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Rename a file in the file manager
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="newName">The new name for the file</param>
        /// <returns>The new path of the file</returns>
        [HttpPost, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult Rename(string path, string newName)
        {
            if (!path.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\")) filePath = filePath.Substring(0, filePath.Length - 1);
                string newFilePath = filePath.UpToLast("\\") + "\\" + newName;
                System.IO.Directory.Move(filePath, newFilePath);
            }
            catch
            {
                return Json("");
            }
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            return Json(path.UpToLast("/") + "/" + newName);
        }

        /// <summary>
        /// Move a file to a different folder
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="newDir">The path of the folder to move it to</param>
        /// <returns>JSON true if succeeds otherwise false</returns>
        [HttpPost, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult Move(string path, string newDir)
        {
            if (!path.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\")) filePath = filePath.Substring(0, filePath.Length - 1);
                string newDirPath = Server.MapPath(newDir);
                if (newDirPath.EndsWith("\\")) newDirPath = newDirPath.Substring(0, newDirPath.Length - 1);
                string newFilePath = newDirPath + "\\" + filePath.LastAfter("\\");
                System.IO.Directory.Move(filePath, newFilePath);
            }
            catch
            {
                return Json(false);
            }
            return Json(true);
        }

        /// <summary>
        /// Delete the file at a given file manager path
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>JSON true if succeeds</returns>
        [HttpPost, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult Delete(string path)
        {
            if (!path.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\"))
                {
                    filePath = filePath.Substring(0, filePath.Length - 1);
                    DirectoryInfo d = new DirectoryInfo(filePath);
                    d.DeleteRecursive();
                }
                else
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error deleting: " + ex.Message);
            }
            return Json(true);
        }

        /// <summary>
        /// Create a new folder in the file manager
        /// </summary>
        /// <param name="path">Path to the folder</param>
        /// <returns>JSON true if succeeds</returns>
        [HttpPost, Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult CreateDir(string path)
        {
            if (!path.StartsWith(ConfigHelper.FileManagerRoot))
                return new HttpStatusCodeResult(403, "Cannot access this directory");

            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\"))
                {
                    filePath = filePath.Substring(0, filePath.Length - 1);
                    DirectoryInfo d = new DirectoryInfo(filePath);
                    d.CreateSubdirectory("New Folder");
                }
                else
                    throw new Exception("Can't create directory in file");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error deleting: " + ex.Message);
            }

            return Json(true);
        }

        /// <summary>
        /// Encrypt a password
        /// </summary>
        /// <param name="pw">The password</param>
        /// <returns>The encrypted password in JSON</returns>
        //[HttpPost, Authorize(Roles = Membership.User.AdminRole)]
        //public ActionResult EncryptPassword(string pw)
        //{
        //    return Json(new { encrypted = LyniconSecurityManager.Current.EncryptPassword(pw) });
        //}

        /// <summary>
        /// Get the possible values for a reference given a partial string typed in by the user
        /// </summary>
        /// <param name="query">The partial string typed in</param>
        /// <param name="listId">A listId which contains possible multiple content types representing the list of possible values</param>
        /// <param name="allowedVsn">Serialized version which indicates which versions are allowed in the list</param>
        /// <returns></returns>
        [Authorize(Roles = Membership.User.EditorRole)]
        public ActionResult RefQuery(string query, string listId, string allowedVsn)
        {
            var types = listId.Split('_').Select(cn => ContentTypeHierarchy.GetContentType(cn)).ToList();
            bool showType = types.Count > 1;
            var qWords = query.ToLower().Split(new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ItemVersion maskVsn = null;
            ItemVersion currMaskedVsn = null;
            bool versioned = false;
            if (!string.IsNullOrEmpty(allowedVsn))
            {
                maskVsn = new ItemVersion(allowedVsn);
                ItemVersion curr = VersionManager.Instance.CurrentVersion;
                ItemVersion vsn = curr.Superimpose(maskVsn);
                currMaskedVsn = curr.Mask(maskVsn);
                VersionManager.Instance.PushState(VersioningMode.Specific, vsn);
                versioned = true;
            }

            try
            {
                var cachedTypes = types.Where(t => Cache.IsTotalCached(t, true)).ToList();
                var uncachedTypes = types.Except(cachedTypes).ToList();
                var items = Enumerable.Range(0, 1).Select(n => new { label = "", value = "" }).ToList();
                items.Clear();
                if (uncachedTypes.Count > 0)
                {
                    // TO DO add attribute for containers specifying which field or fields to scan for title, add code to create query to scan here
                }
                if (cachedTypes.Count > 0)
                {
                    items.AddRange(Collator.Instance.Get<Summary, Summary>(cachedTypes,
                        iq => iq.Where(s => qWords.All(w => ((s.Title ?? "").ToLower() + " " + s.Type.Name.ToLower()).Contains(w))).Take(30))                
                        .Select(summ => new
                        {
                            label = summ.Title + (showType ? " (" + BaseContent.ContentClassDisplayName(summ.Type) + ")" : "")
                                    + (versioned && !currMaskedVsn.ContainedBy(summ.Version.Mask(maskVsn)) ? " [" + VersionManager.Instance.DisplayVersion(summ.Version.Mask(maskVsn)).Select(dv => dv.Text).Join(" ") + "]" : ""),
                            value = versioned ? summ.ItemVersionedId.Mask(maskVsn).ToString() : summ.ItemId.ToString()
                        })
                        .OrderBy(s => s.label));
                }

                return Json(new { items }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (versioned)
                    VersionManager.Instance.PopState();
            }
        }
    }
}
