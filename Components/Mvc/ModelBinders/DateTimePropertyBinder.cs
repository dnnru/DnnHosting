#region

using System;
using System.ComponentModel;
using System.Web.Mvc;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    public class DateTimePropertyBinder : IPropertyBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + memberDescriptor.Name);
            if (valueResult == null /* && bindingContext.FallbackToEmptyPrefix*/)
            {
                valueResult = bindingContext.ValueProvider.GetValue(memberDescriptor.Name);
            }

            DateTime? result = valueResult?.RawValue.ToDateTime();
            return result ?? DateTime.MinValue;
        }
    }
}