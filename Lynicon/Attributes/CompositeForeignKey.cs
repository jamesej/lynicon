using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Marks a property as not to be included in type compositing (but used in entity framework elsewhere)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CompositeForeignKeyAttribute : NotMappedAttribute
    {
        public string FKFieldName { get; set; }
        public CompositeForeignKeyAttribute(string fkFieldName)
        {
            FKFieldName = fkFieldName;
        }
    }
}
