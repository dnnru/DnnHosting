using System.Collections.Generic;

namespace Italliance.Modules.DnnHosting.Components.Resource
{
    public interface ILocateResources
    {
        ResourceReference Locate(string name);
        IEnumerable<ResourceEntry> EnumerateResourceEntries();
    }
}