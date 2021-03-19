#region

using System.ComponentModel;
using System.Web.Mvc;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    public class NullableDateTimePropertyBinder : IPropertyBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + memberDescriptor.Name);
            if (valueResult == null /* && bindingContext.FallbackToEmptyPrefix*/)
            {
                valueResult = bindingContext.ValueProvider.GetValue(memberDescriptor.Name);
            }

            return valueResult?.RawValue.ToDateTime();
        }
    }
}