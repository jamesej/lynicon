using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Lynicon.Models;

namespace Lynicon.Binding
{
    public class RecaptchaBinder : IModelBinder
    {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult chall = bindingContext.ValueProvider.GetValue("recaptcha_challenge_field");
            ValueProviderResult resp = bindingContext.ValueProvider.GetValue("recaptcha_response_field");
            if (chall != null && resp != null && !string.IsNullOrEmpty(chall.AttemptedValue) && !string.IsNullOrEmpty(resp.AttemptedValue))
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName + ".recaptcha_challenge_field", chall);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName + ".recaptcha_response_field", resp);

                Recaptcha rec = new Recaptcha
                {
                    recaptcha_challenge_field = ((string[])chall.RawValue)[0],
                    recaptcha_response_field = ((string[])resp.RawValue)[0]
                };

                try
                {
                    if (!rec.Verify())
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "You typed the pictured text incorrectly, please try again");
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "We could not validate you typed the pictured words correctly, please try again");
                }

                return rec;
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Please type the pictured text into the box underneath it");
                return null;
            }
        }

        #endregion
    }
}
