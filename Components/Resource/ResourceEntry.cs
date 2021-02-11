using System.Reflection;

namespace Italliance.Modules.DnnHosting.Components.Resource
{
    public class ResourceEntry
    {
        public Assembly Assembly { get; set; }
        public string ManifestResourceName { get; set; }
        public string Name { get; set; }
    }
}