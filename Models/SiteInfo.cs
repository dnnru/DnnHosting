#region

using System.Collections.Generic;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    public class SiteInfo
    {
        public SiteInfo()
        {
            Bindings = new List<string>();
        }

        public string Key { get; set; }
        public string SiteName { get; set; }
        public string ConnectionString { get; set; }
        public string ObjectQualifier { get; set; }
        public string DatabaseOwner { get; set; }
        public List<string> Bindings { get; }
    }
}