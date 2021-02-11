#region Usings

using System;
using DotNetNuke.Entities.Modules.Settings;
using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Components.Resource;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    [Serializable]
    public class DnnHostingSettings
    {
        [PortalSetting(Prefix = "DnnHosting_")]
        public int DaysToExpire { get; set; } = 10;

        [PortalSetting(Prefix = "DnnHosting_")]
        public string AdminEmail { get; set; } = "admin@italliance.kz";

        [PortalSetting(Prefix = "DnnHosting_")]
        public string IisAdministrationUrl { get; set; } = "https://localhost:55539";
        
        [PortalSetting(Prefix = "DnnHosting_")]
        public string IisAdministrationApiKey { get; set; }

        [PortalSetting(Prefix = "DnnHosting_")]
        public string EmailFrom { get; set; } = "hosting@italliance.kz";

        [PortalSetting(Prefix = "DnnHosting_")]
        public string AdminEmailSubject { get; set; } = "Отчет о просроченных или требующих оплаты клиентах хостинга.";

        [PortalSetting(Prefix = "DnnHosting_")]
        public string AdminEmailTemplate { get; set; } = Utils.EmailTemplates.Admin;

        [PortalSetting(Prefix = "DnnHosting_")]
        public string ClientEmailSubject { get; set; } = "Уведомление о необходимости оплаты хостинга для домена {0}";

        [PortalSetting(Prefix = "DnnHosting_")]
        public string ClientEmailTemplate { get; set; } = Utils.EmailTemplates.Client;
        
        [PortalSetting(Prefix = "DnnHosting_")]
        public string ClientPartialEmailTemplate { get; set; } = Utils.EmailTemplates.ClientPartial;

        [PortalSetting(Prefix = "DnnHosting_")]
        public int PageSize { get; set; } = 12;
    }
}
