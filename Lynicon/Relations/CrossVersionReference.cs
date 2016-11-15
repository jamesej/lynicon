using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Relations;
using Newtonsoft.Json;

namespace Lynicon.Relations
{
    /// <summary>
    /// A reference which can hold references to content items in other versions
    /// </summary>
    /// <typeparam name="T">A type to which the content type of the referred to item must be assignable</typeparam>
    [Serializable]
    public class CrossVersionReference<T> : Reference<T>, ICrossVersionReference where T : class
    {
        protected ItemVersion version = new ItemVersion();
        /// <summary>
        /// The version of the referred to content item
        /// </summary>
        public virtual ItemVersion Version
        {
            get { return version; }
            set { version = value == null ? null : value.Mask(AllowedVersionsOverlay ?? new ItemVersion()); }
        }

        /// <summary>
        /// The ItemVersionedId of the referred to content item
        /// </summary>
        [JsonIgnore, ScaffoldColumn(false)]
        public override ItemVersionedId VersionedId
        {
            get
            {
                if (Id == null || string.IsNullOrEmpty(DataType))
                    return null;
                var vsn = this.Version ?? new ItemVersion();
                return new ItemVersionedId(this.ItemId, vsn);
            }
            set
            {
                Id = value.Id.ToString();
                DataType = value.Type.FullName;
                Version = value.Version;
            }
        }

        /// <summary>
        /// A version key set to 'null' will allow the reference to be set to any version of that key.
        /// A version key set to a value will mean the reference is ALWAYS to that version, irrespective of the current version.
        /// </summary>
        public virtual ItemVersion AllowedVersionsOverlay { get; set; }

        /// <summary>
        /// Create an empty CrossVersionReference
        /// </summary>
        public CrossVersionReference() : base()
        { }
        /// <summary>
        /// Create a CrossVersionReference with an ItemVersion which has an AllowedVersionsOverlay to limit the versions which
        /// can be referred to
        /// </summary>
        /// <param name="allowedVersionsOverlay">A version key set to 'null' will allow the reference to be set to any version of that key. A version key set to a value will mean the reference is ALWAYS to that version, irrespective of the current version.</param>
        public CrossVersionReference(ItemVersion allowedVersionsOverlay)
            : this()
        {
            AllowedVersionsOverlay = allowedVersionsOverlay;
        }
        /// <summary>
        /// Create a CrossVersionReference from an ItemVersionedId
        /// </summary>
        /// <param name="ivid">ItemVersionedId for the referred to item</param>
        public CrossVersionReference(ItemVersionedId ivid) : base(ivid)
        {
            VersionedId = ivid;
        }
        /// <summary>
        /// Create a CrossVersionReference with an ItemVersion which has an AllowedVersionsOverlay to limit the versions which
        /// can be referred to, and the ItemVersionedId of the referred to item
        /// </summary>
        /// <param name="allowedVersionsOverlay">A version key set to 'null' will allow the reference to be set to any version of that key. A version key set to a value will mean the reference is ALWAYS to that version, irrespective of the current version.</param>
        /// <param name="ivid">ItemVersionedId for the referred to item</param>
        public CrossVersionReference(ItemVersion allowedVersionsOverlay, ItemVersionedId ivid)
            : this(ivid)
        {
            AllowedVersionsOverlay = allowedVersionsOverlay;
        }
        /// <summary>
        /// Create a CrossVersionReference from a serialized string for the ItemVersionedId for the referred to item
        /// </summary>
        /// <param name="id">The serialized ItemVersionedId for the referred to item</param>
        public CrossVersionReference(string id) : base(new ItemVersionedId(id))
        {
            VersionedId = new ItemVersionedId(id);
        }
        /// <summary>
        /// Create a CrossVersionReference with an ItemVersion which has an AllowedVersionsOverlay to limit the versions which
        /// can be referred to, and a serialized string of the ItemVersionedId of the referred to item
        /// </summary>
        /// <param name="allowedVersionsOverlay">A version key set to 'null' will allow the reference to be set to any version of that key. A version key set to a value will mean the reference is ALWAYS to that version, irrespective of the current version.</param>
        /// <param name="id">The serialized ItemVersionedId for the referred to item</param>
        public CrossVersionReference(ItemVersion allowedVersionsOverlay, string id) : this(id)
        {
            AllowedVersionsOverlay = allowedVersionsOverlay;
        }
        /// <summary>
        /// Create a CrossVersionReference from the name of the data type, the identity of the item and the version of the item
        /// </summary>
        /// <param name="dataType">The data type of the referred to item</param>
        /// <param name="id">The identity of the referred to item</param>
        /// <param name="itemVersion">The ItemVersion of the referred to item</param>
        public CrossVersionReference(string dataType, string id, string itemVersion) : base(dataType, id)
        {
            Version = new ItemVersion(itemVersion);
        }
        /// <summary>
        /// Create a CrossVersionReference from the name of the data type, the identity of the item and the version of the item,
        /// which has an AllowedVersionsOverlay to limit the versions which can be referred to
        /// </summary>
        /// <param name="allowedVersionsOverlay">A version key set to 'null' will allow the reference to be set to any version of that key. A version key set to a value will mean the reference is ALWAYS to that version, irrespective of the current version.</param>
        /// <param name="dataType">The data type of the referred to item</param>
        /// <param name="id">The identity of the referred to item</param>
        /// <param name="itemVersion">The ItemVersion of the referred to item</param>
        public CrossVersionReference(ItemVersion allowedVersionsOverlay, string dataType, string id, string itemVersion)
            : base(dataType, id)
        {
            AllowedVersionsOverlay = allowedVersionsOverlay;
            Version = Version.Mask(allowedVersionsOverlay);
        }

        public override string SerializedValue
        {
            get
            {
                return ItemId == null ? "" : VersionedId.ToString();
            }
            set
            {
                var ivid = new ItemVersionedId(value);
                if (ivid != null && ivid.Id != null)
                {
                    this.Id = ivid.Id.ToString();
                    this.DataType = ivid.Type.FullName;
                    this.Version = ivid.Version;
                    this.summary = null; // force reload of summary
                }
                else
                {
                    this.Id = null;
                    this.DataType = null;
                    this.Version = null;
                    this.summary = null;
                }
            }
        }

        public override Summary Summary
        {
            get
            {
                if (Id == null || string.IsNullOrEmpty(DataType) || VersionedId == null)
                    return null;
                if (summary == null && ItemId != null)
                {
                    ItemVersion vsn = VersionManager.Instance.CurrentVersion.Superimpose(this.Version);
                    VersionManager.Instance.PushState(VersioningMode.Specific, vsn);
                    try
                    {
                        summary = Collator.Instance.Get<Summary>(ItemId);
                    }
                    finally
                    {
                        VersionManager.Instance.PopState();
                    }
                }
                return summary;
            }
        }

        public override List<SelectListItem> GetSelectList()
        {
            ItemVersion getVsn = VersionManager.Instance.CurrentVersion.Overlay(AllowedVersionsOverlay);
            VersionManager.Instance.PushState(VersioningMode.Specific, getVsn);
            try
            {
                var summaries = Collator.Instance.Get<Summary, object>(AssignableContentTypes, iq => iq);
                var slis = summaries.Select(s => new SelectListItem
                {
                    Text = IsContentType ? s.Title : s.Title + " (" + BaseContent.ContentClassDisplayName(s.Type) + ")",
                    Value = s.ItemVersionedId.Mask(AllowedVersionsOverlay).ToString(),
                    Selected = (s.Id.ToString() == this.Id && s.Type == this.Type && s.Version.Mask(AllowedVersionsOverlay) == this.Version)
                }).OrderBy(sli => sli.Text).ToList();
                bool noneSelected = !slis.Any(sli => sli.Selected);
                slis.Insert(0, new SelectListItem { Text = "", Value = "", Selected = noneSelected });
                return slis;
            }
            finally
            {
                VersionManager.Instance.PopState();
            }
        }
    }
}
