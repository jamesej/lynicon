using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Relations;
using Lynicon.Utility;

namespace Lynicon.Relations
{
    /// <summary>
    /// Interface for a list of references
    /// </summary>
    public interface IReferenceList
    {
        string SerializedValue { get; set; }
        string SelectListId();
        List<SelectListItem> GetSelectList();
    }

    /// <summary>
    /// Model binding for a reference list (uses SerializedValue)
    /// </summary>
    public class ReferenceListModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IReferenceList refList = Activator.CreateInstance(bindingContext.ModelType) as IReferenceList;
            var attVal = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".SerializedValue");
            refList.SerializedValue = attVal == null ? null : attVal.AttemptedValue;

            return refList;
        }
    }

    /// <summary>
    /// List of typed references
    /// </summary>
    /// <typeparam name="T">A type to which the content type of the referred to item must be assignable</typeparam>
    [Serializable, ModelBinder(typeof(ReferenceListModelBinder))]
    public class ReferenceList<T> : List<Reference<T>>, IReferenceList where T : class
    {
        /// <summary>
        /// Serialized value of the list of references
        /// </summary>
        public string SerializedValue
        {
            get
            {
                return this.Select(r => r.SerializedValue).Join(",");
            }
            set
            {
                this.Clear();
                value.Split(',')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Do(s => this.Add(new Reference<T> { SerializedValue = s.Trim() }));
            }
        }

        /// <summary>
        /// Select list id for type T
        /// </summary>
        /// <returns>Unique id for the select list</returns>
        public string SelectListId()
        {
            return new Reference<T>().SelectListId();
        }

        /// <summary>
        /// Select list for type T
        /// </summary>
        /// <returns>Select list for each reference item in the list</returns>
        public List<SelectListItem> GetSelectList()
        {
            return new Reference<T>().GetSelectList();
        }
    }
}
