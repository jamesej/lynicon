using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Mvc;
using Lynicon.Models;
using Lynicon.Attributes;
using Lynicon.Utility;

namespace Lynicon.Binding
{
    public class LyniconBinder : DefaultModelBinder
    {
        public LyniconBinder()
        {
            // Use this as the default binder in the scope of BindModel on this binder only
            var binders = new ModelBinderDictionary();
            ModelBinders.Binders.Do(kvp => binders.Add(kvp.Key, kvp.Value));
            binders.DefaultBinder = this;
            this.Binders = binders;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            // Check for existence of type discriminator field
            string typeDiscrimKey = CreateSubPropertyName(bindingContext.ModelName, "_TYPEDISC_");
            ValueProviderResult vpDiscrim = bindingContext.ValueProvider.GetValue(typeDiscrimKey);
            if (vpDiscrim != null)
            {
                // check for attribute on property specifying the requested type name is allowed
                string typeName = (string)vpDiscrim.ConvertTo(typeof(string));
                var attr = propertyDescriptor.Attributes.OfType<AllowedSubtypesAttribute>().FirstOrDefault();
                if (attr != null && attr.AllowedSubtypeNames.Contains(typeName))
                {
                    // check if the requested type is different from the property type, but assignable to it
                    Type propType = Type.GetType(typeName);
                    if (propType != propertyDescriptor.PropertyType && propertyDescriptor.PropertyType.IsAssignableFrom(propType))
                    {
                        // substitute type of property for specified type
                        IModelBinder newPropertyBinder = Binders.GetBinder(propType);
                        var propertyMetadata =
                            ModelMetadataProviders.Current.GetMetadataForType(() => null, propType);
                        ModelBindingContext newBindingContext = new ModelBindingContext()
                        {
                            ModelMetadata = propertyMetadata,
                            ModelName = bindingContext.ModelName,
                            ModelState = bindingContext.ModelState,
                            ValueProvider = bindingContext.ValueProvider
                        };

                        return base.GetPropertyValue(controllerContext, newBindingContext, propertyDescriptor, newPropertyBinder);
                    }
                }
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}
