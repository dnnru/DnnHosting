using System.ComponentModel;
using System.Web.Mvc;

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    interface IPropertyBinder
    {
        object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, MemberDescriptor memberDescriptor);
    }
}