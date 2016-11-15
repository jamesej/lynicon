using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Membership;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Utility;
using System.Collections;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// Describes the conditions under which content operations may be performed
    /// depending on the current user's roles and the content item itself
    /// </summary>
    public class ContentPermission
    {
        /// <summary>
        /// Function which returns true if the supplied list of roles is permitted
        /// </summary>
        public Func<string, bool> RolesPermitted { get; set; }
        /// <summary>
        /// Function which returns true if the given content type is permitted
        /// </summary>
        public Func<Type, bool> TypePermitted { get; set; }
        /// <summary>
        /// Function which returns true if the given content item is permitted
        /// </summary>
        public Func<object, bool> ContentPermitted { get; set; }

        /// <summary>
        /// Dictionary whose keys are version keys and whose values are lists of
        /// valid version values.  For every key in the content item's version, if that
        /// key is in the VersionMask, the value must match one of the values in the
        /// VersionMask
        /// </summary>
        public Dictionary<string, List<object>> VersionMask { get; set; }

        /// <summary>
        /// Create a ContentPermission that always permits the content item
        /// </summary>
        public ContentPermission()
        {
            RolesPermitted = s => true;
            TypePermitted = t => true;
            ContentPermitted = c => true;
            VersionMask = new Dictionary<string, List<object>>();
        }
        /// <summary>
        /// Create a ContentPermission that requires all the roles listed
        /// </summary>
        /// <param name="requiredRoles">string containing all the role letters required</param>
        public ContentPermission(string requiredRoles) : this()
        {
            RolesPermitted = roles => requiredRoles.All(rc => roles.Contains(rc));
        }
        /// <summary>
        /// Create a ContentPermission with a list of required roles, a test for a permitted type, and a test for
        /// a permitted content item
        /// </summary>
        /// <param name="requiredRoles">string containing all the role letters required</param>
        /// <param name="typePermitted">function returning true if a type is permitted</param>
        /// <param name="contentPermitted">function returning true if a content item is permitted</param>
        public ContentPermission(string requiredRoles, Func<Type, bool> typePermitted, Func<object, bool> contentPermitted)
            : this(requiredRoles)
        {
            TypePermitted = typePermitted;
            ContentPermitted = contentPermitted;
        }

        private string CurrentRoles()
        {
            IUser u = LyniconSecurityManager.Current.User;
            if (u == null) return "";
            return u.Roles;
        }

        private bool VersionPermitted(object content)
        {
            if (VersionMask.Keys.Count == 0)
                return true;

            Type type = null;
            if (content is IList)
                type = content.GetType().GetGenericArguments()[0];
            else if (content != null)
                type = content.GetType().ContentType();

            if (content == null || !ContentTypeHierarchy.AllContentTypes.Contains(type))
                return false;

            ItemVersion vsn = null;
            if (content is IList)
            {
                vsn = VersionManager.Instance.CurrentVersion;
            }
            else
            {
                var container = Collator.Instance.GetContainer(content);
                if (container == null)
                    return false;

                vsn = new ItemVersion(container);
            }

            foreach (var kvp in vsn)
            {
                if (VersionMask.ContainsKey(kvp.Key))
                    if (!VersionMask[kvp.Key].Contains(kvp.Value))
                        return false;
            }
            return true;
        }

        /// <summary>
        /// Test whether a given content item is permitted under this ContentPermission
        /// </summary>
        /// <param name="content">The content item</param>
        /// <returns>True if permitted</returns>
        public bool Permitted(object content)
        {
            bool rolesOK = RolesPermitted(CurrentRoles());
            bool typeOK = TypePermitted(content == null ? typeof(object) : content.GetType().ContentType());
            bool contentOK = ContentPermitted(content);
            bool versionOK = VersionPermitted(content);
            return rolesOK && typeOK && contentOK && versionOK;
        }
    }
}
