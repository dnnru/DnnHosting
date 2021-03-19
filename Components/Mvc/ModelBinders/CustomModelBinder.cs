#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    public class CustomModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var propertyBinderAttribute = TryFindPropertyBinderAttribute(propertyDescriptor);
            if (propertyBinderAttribute != null)
            {
                var binder = CreateBinder(propertyBinderAttribute);
                var value = binder.BindModel(controllerContext, bindingContext, propertyDescriptor);
                propertyDescriptor.SetValue(bindingContext.Model, value);
            }
            else // revert to the default behavior.
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private IPropertyBinder CreateBinder(PropertyBinderAttribute propertyBinderAttribute)
        {
            //return (IPropertyBinder) DependencyResolver.Current.GetService(propertyBinderAttribute.BinderType);
            return (IPropertyBinder) Activator.CreateInstance(propertyBinderAttribute.BinderType, true);
        }

        private PropertyBinderAttribute TryFindPropertyBinderAttribute(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.Attributes.OfType<PropertyBinderAttribute>().FirstOrDefault();
        }
    }
}