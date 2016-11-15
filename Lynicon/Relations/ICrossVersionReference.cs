using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Collation;

namespace Lynicon.Relations
{
    /// <summary>
    /// Applied to a container which supports basic auditing metadata
    /// </summary>
    public interface ICrossVersionReference
    {
        /// <summary>
        /// The version of the referred to item
        /// </summary>
        ItemVersion Version { get; set; }
        /// <summary>
        /// The overlay for restricting what versions can be referred to
        /// </summary>
        ItemVersion AllowedVersionsOverlay { get; }
        /// <summary>
        /// The reference serialized to a string
        /// </summary>
        string SerializedValue { get; set; }
        /// <summary>
        /// Summary of the content item referred to
        /// </summary>
        Summary Summary { get; }
        /// <summary>
        /// A select list for selecting values for the reference
        /// </summary>
        /// <returns>List of possible values</returns>
        List<SelectListItem> GetSelectList();

        /// <summary>
        /// ItemVersionedId of the referred to item
        /// </summary>
        ItemVersionedId VersionedId { get; }
    }
}
