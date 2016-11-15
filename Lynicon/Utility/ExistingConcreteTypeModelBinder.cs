using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Utility
{
    /// <summary>
    /// A model binder which uses the concrete type of data in the model rather than the declared type
    /// </summary>
    public class ExistingConcreteTypeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.IsAbstract && bindingContext.Model != null)
            {
                var concreteType = bindingContext.Model.GetType();

                if (Nullable.GetUnderlyingType(concreteType) == null)
                {
                    return new AssociatedMetadataTypeTypeDescriptionProvider(concreteType).GetTypeDescriptor(concreteType);
                }
            }

            return base.GetTypeDescriptor(controllerContext, bindingContext);
        }
    }
}
