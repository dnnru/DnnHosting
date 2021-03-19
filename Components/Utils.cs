#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using Italliance.Modules.DnnHosting.Components.Resource;
using Italliance.Modules.DnnHosting.Models;
using LinqToDB;
using LinqToDB.Tools;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public static class Utils
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<IEmailTemplates> _emailTemplates;

        static Utils()
        {
            _emailTemplates = new Lazy<IEmailTemplates>(GetEmailTemplates);
        }

        public static IEmailTemplates EmailTemplates => _emailTemplates.Value;

        private static IEmailTemplates GetEmailTemplates()
        {
            AssemblyResourceLocator locator = new AssemblyResourceLocator(typeof(EmailTemplates).Assembly);
            EmbeddedResourceLoader loader = new EmbeddedResourceLoader(locator);
            EmailTemplates templates = new EmailTemplates
                                       {
                                           Admin = loader.LoadText("Admin.html"), Client = loader.LoadText("Client.html"),
                                           ClientPartial = loader.LoadText("ClientPartial.html")
                                       };
            return templates;
        }

        public static bool UpdatePortals(List<SiteInfo> siteInfos, Client client, out string error)
        {
            error = "";
            try
            {
                string[] domains = client.Domain.Split(new[] {';', ','}, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()).ToArray();
                List<SiteInfo> currentSites = new List<SiteInfo>();
                foreach (string domain in domains)
                {
                    currentSites.AddRange(siteInfos.FindAll(s => s.Bindings.Contains(domain, StringComparer.OrdinalIgnoreCase)));
                }

                foreach (SiteInfo currentSite in currentSites.Where(currentSite => !string.IsNullOrWhiteSpace(currentSite.ConnectionString)
                                                                                && !string.IsNullOrWhiteSpace(currentSite.DatabaseOwner)))
                {
                    using (DnnDataConnection connection = new DnnDataConnection(currentSite.ConnectionString, currentSite.DatabaseOwner, currentSite.ObjectQualifier))
                    {
                        List<int> portalIds = connection.PortalAliases.Where(pa => pa.HTTPAlias.In(domains)).Select(pa => pa.PortalID).ToList();
                        foreach (Portal portal in portalIds.Select(id => connection.Portals.FirstOrDefault(p => p.PortalID == id)).Where(portal => portal != null))
                        {
                            portal.ExpiryDate = client.HostingEndDate;
                            portal.HostSpace = client.HostSpace;
                            portal.PageQuota = client.PageQuota;
                            portal.UserQuota = client.UserQuota;

                            connection.Update(portal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }

            return true;
        }

        public static IEnumerable<ModuleInfo> GetDnnHostingModules()
        {
            ModuleDefinitionInfo definitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(Constants.MODULE_FRIENDLY_NAME);
            if (definitionInfo != null)
            {
                List<ModuleInfo> modules = new List<ModuleInfo>();
                foreach (PortalInfo portal in PortalController.Instance.GetPortalList(Null.NullString))
                {
                    ModuleInfo moduleInfo = ModuleController.Instance.GetModuleByDefinition(portal.PortalID, definitionInfo.DefinitionName);
                    if (moduleInfo != null)
                    {
                        modules.Add(moduleInfo);
                    }
                }

                return modules;
            }

            return null;
        }

        public static string ToInvariantString(this object obj)
        {
            return obj is IConvertible convertible ? convertible.ToString(CultureInfo.InvariantCulture) :
                   obj is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : obj.ToString();
        }

        public static string ToCultureSpecificString(this object obj, CultureInfo cultureInfo)
        {
            return cultureInfo == null ? obj.ToInvariantString() :
                   obj is IConvertible convertible ? convertible.ToString(cultureInfo) :
                   obj is IFormattable formattable ? formattable.ToString(null, cultureInfo) : obj.ToString();
        }

        public static short ToShort(this string s, short defaultValue = 0)
        {
            return short.TryParse(s, out short v) ? v : defaultValue;
        }

        public static int ToInt(this string s, int defaultValue = 0)
        {
            return int.TryParse(s, out int v) ? v : defaultValue;
        }

        public static long ToLong(this string s, long defaultValue = 0)
        {
            return long.TryParse(s, out long v) ? v : defaultValue;
        }

        public static decimal ToDecimal(this string s, decimal defaultValue = 0)
        {
            return decimal.TryParse(s, out decimal v) ? v : defaultValue;
        }

        public static float ToFloat(this string s, float defaultValue = 0)
        {
            return float.TryParse(s, out float v) ? v : defaultValue;
        }

        public static double ToDouble(this string s, double defaultValue = 0)
        {
            return double.TryParse(s, out double v) ? v : defaultValue;
        }

        public static bool ToBoolean(this string s, bool defaultValue = false)
        {
            return bool.TryParse(s, out bool v) ? v : defaultValue;
        }

        public static short? ToShortNullable(this string s, short? defaultValue = null)
        {
            return short.TryParse(s, out short v) ? v : defaultValue;
        }

        public static int? ToIntNullable(this string s, int? defaultValue = null)
        {
            return int.TryParse(s, out int v) ? v : defaultValue;
        }

        public static long? ToLongNullable(this string s, long? defaultValue = null)
        {
            return long.TryParse(s, out long v) ? v : defaultValue;
        }

        public static decimal? ToDecimalNullable(this string s, decimal? defaultValue = null)
        {
            return decimal.TryParse(s, out decimal v) ? v : defaultValue;
        }

        public static float? ToFloatNullable(this string s, float? defaultValue = null)
        {
            return float.TryParse(s, out float v) ? v : defaultValue;
        }

        public static double? ToDoubleNullable(this string s, double? defaultValue = null)
        {
            return double.TryParse(s, out double v) ? v : defaultValue;
        }

        public static bool? ToBooleanNullable(this string s, bool? defaultValue = null)
        {
            return bool.TryParse(s, out bool v) ? v : defaultValue;
        }

        public static bool TryConvertToEnum<T>(this int instance, out T result) where T : Enum
        {
            var enumType = typeof(T);
            var success = Enum.IsDefined(enumType, instance);
            if (success)
            {
                result = (T) Enum.ToObject(enumType, instance);
            }
            else
            {
                result = default;
            }

            return success;
        }

        public static T ToEnum<T>(this int instance) where T : Enum
        {
            instance.TryConvertToEnum(out T result);
            return result;
        }
    }
}