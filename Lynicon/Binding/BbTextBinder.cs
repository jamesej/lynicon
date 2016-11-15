using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Models;

namespace L24CM.Binding
{
    public class BbTextBinder : IModelBinder
    {

        public BbTextBinder() { }
 
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
     
            return bindingContext.
        }
 
    private Nullable<T> GetA<T>(ModelBindingContext bindingContext, string key) where T : struct
    {
        if (String.IsNullOrEmpty(key)) return null;
        ValueProviderResult valueResult;
        //Try it with the prefix...
        bindingContext.ValueProvider.TryGetValue(bindingContext.ModelName + "." + key, out valueResult);
        //Didn't work? Try without the prefix if needed...
        if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
        {
            bindingContext.ValueProvider.TryGetValue(key, out valueResult);
        }
        if (valueResult == null)
        {
            return null;
        }
        return (Nullable<T>)valueResult.ConvertTo(typeof(T));
    }

    }
}
