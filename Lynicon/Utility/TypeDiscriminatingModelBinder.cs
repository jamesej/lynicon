using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Utility
{
    /// <summary>
    /// A model binder which uses a 'fake' field key (e.g. name attribute of html input) which is the path
    /// for the property plus '.ModelType' which is the name of the underlying type of the data
    /// allowing for creating of values of a different underlying type to the declared type of the
    /// property holding them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeDiscriminatingModelBinder<T> : DefaultModelBinder where T : class
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            // Get the type for this element of the model
            var typeValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".ModelType");
            var type = Type.GetType(
                (string)typeValue.ConvertTo(typeof(string)),
                true
            );
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Bad Type");
            }

            // create an instance of the type to be the model
            var model = Activator.CreateInstance(type);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, type);
            return model;
        }
    }
}
