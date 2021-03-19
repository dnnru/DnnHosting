#region

using System;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders
{
    public class PropertyBinderAttribute : Attribute
    {
        public PropertyBinderAttribute(Type binderType)
        {
            BinderType = binderType;
        }

        public Type BinderType { get; }
    }
}