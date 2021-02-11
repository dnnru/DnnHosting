using System.Reflection;

namespace Italliance.Modules.DnnHosting.Components.Resource
{
    public class ResourceReference
    {
        public Assembly Assembly { get; }
        public string FullName { get; }

        public ResourceReference(Assembly assembly, string fullName)
        {
            Assembly = assembly;
            FullName = fullName;
        }
    }
}
