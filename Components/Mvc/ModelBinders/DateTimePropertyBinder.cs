#region

using System;
using System.ComponentModel;
using System.Web.Mvc;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    public class DateTimeModelBinder : IPropertyBinder
    {


        private T? GetA<T>(ModelBindingContext bindingContext, string key) where T : struct
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix)
            {
                valueResult = bindingContext.ValueProvider.GetValue(key);
            }

            return (T?) valueResult?.ConvertTo(typeof(T));
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor)
        {
            throw new NotImplementedException();
        }
    }
}