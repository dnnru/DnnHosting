using System.IO;

namespace Italliance.Modules.DnnHosting.Components.Resource
{
    public interface ILoadResources
    {
        string LoadText(string name);
        byte[] LoadBytes(string name);
        Stream OpenStream(string name);
    }
}