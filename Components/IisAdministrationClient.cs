#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using Italliance.Modules.DnnHosting.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
//using RestSharp.Authenticators;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public class IisAdministrationClient : RestClient
    {
        private const string WEB_SITES_URL = @"/api/webserver/websites";
        private const string ACCESS_TOKEN_HEADER_NAME = @"Access-Token";
        private const string ACCESS_TOKEN_HEADER_VALUE = "Bearer {0}";
        private const string ACCEPT_HEADER_NAME = "Accept";
        private const string ACCEPT_HEADER_VALUE = "application/hal+json";
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(IisAdministrationClient));
        private const string IIS_ADMINISTRATION_CLIENT_CACHE_KEY = "Dnn_DnnHosting_IisAdministration_SiteInfos";

        public IisAdministrationClient(string baseUrl, string accessToken/*, string user, string password*/) : base(baseUrl)
        {
            AccessToken = accessToken;
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            //Authenticator = new NtlmAuthenticator(user, password);
        }

        public string AccessToken { get; }

        private IRestRequest CreateRequest(string url)
        {
            IRestRequest request = new RestRequest(url);
            request.AddHeader(ACCESS_TOKEN_HEADER_NAME, string.Format(ACCESS_TOKEN_HEADER_VALUE, AccessToken));
            request.AddHeader(ACCEPT_HEADER_NAME, ACCEPT_HEADER_VALUE);
            return request;
        }

        private JObject ExecuteJson(IRestRequest request)
        {
            try
            {
                IRestResponse response = Execute(request, Method.GET);
                if (response.IsSuccessful)
                {
                    return JObject.Parse(response.Content);
                }

                Logger.Error($@"Status: '{response.StatusCode} - {response.StatusDescription}'; Error: '{response.ErrorMessage}'");
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("IisAdministrationClient.ExecuteJson()", e);
            }

            return null;
        }

        public IEnumerable<SiteInfo> GetSiteInfosCached()
        {
            List<SiteInfo> siteInfos = DataCache.GetCache<IEnumerable<SiteInfo>>(IIS_ADMINISTRATION_CLIENT_CACHE_KEY)?.ToList();
            if (siteInfos != null && siteInfos.Any())
            {
                return siteInfos;
            }

            siteInfos = GetSiteInfos().ToList();
            DataCache.SetCache(IIS_ADMINISTRATION_CLIENT_CACHE_KEY, siteInfos, DateTime.Now.AddHours(3));

            return siteInfos;
        }

        public IEnumerable<SiteInfo> GetSiteInfos()
        {
            List<SiteInfo> siteInfos = new List<SiteInfo>();
            JObject websitesResponse = ExecuteJson(CreateRequest(WEB_SITES_URL)) ?? throw new ArgumentNullException($"ExecuteJson on {WEB_SITES_URL}");

            List<string> sitesHrefs = websitesResponse.SelectTokens("$.websites[*]", false)
                                                      .Select(jToken => (jToken.SelectToken("$._links.self.href") ?? "").Value<string>())
                                                      .Where(href => !string.IsNullOrWhiteSpace(href))
                                                      .ToList();

            foreach (string href in sitesHrefs)
            {
                JObject siteResponse = ExecuteJson(CreateRequest(href));
                if (siteResponse == null)
                {
                    continue;
                }

                SiteInfo siteInfo = new SiteInfo
                                    {
                                        SiteName = (siteResponse.SelectToken("$.name") ?? "").Value<string>(),
                                        Key = (siteResponse.SelectToken("$.key") ?? "").Value<string>()
                                    };
                foreach (JToken jToken in siteResponse.SelectTokens("$.bindings[*]", false))
                {
                    string hostname = (jToken.SelectToken("$.hostname") ?? "").Value<string>();
                    if (!string.IsNullOrWhiteSpace(hostname))
                    {
                        siteInfo.Bindings.Add(hostname);
                    }
                }

                string filesHref = (siteResponse.SelectToken("$._links.files.href") ?? "").Value<string>();
                if (string.IsNullOrWhiteSpace(filesHref))
                {
                    continue;
                }

                JObject filesResponse = ExecuteJson(CreateRequest(filesHref));
                if (filesResponse == null)
                {
                    continue;
                }

                string siteFilesHref = (filesResponse.SelectToken("$._links.files.href") ?? "").Value<string>();
                if (string.IsNullOrWhiteSpace(siteFilesHref))
                {
                    continue;
                }

                JObject siteFilesResponse = ExecuteJson(CreateRequest(siteFilesHref));
                if (siteFilesResponse == null)
                {
                    continue;
                }

                foreach (JToken jToken in siteFilesResponse.SelectTokens("$.files[*]"))
                {
                    string fileName = (jToken.SelectToken("$.name") ?? "").Value<string>();
                    if (string.IsNullOrWhiteSpace(fileName) || fileName.Trim().ToLower() != "web.config")
                    {
                        continue;
                    }

                    string webConfigHref = (jToken.SelectToken("$._links.self.href") ?? "").Value<string>();
                    if (string.IsNullOrWhiteSpace(webConfigHref))
                    {
                        continue;
                    }

                    JObject webConfigResponse = ExecuteJson(CreateRequest(webConfigHref));
                    if (webConfigResponse == null)
                    {
                        continue;
                    }

                    string webConfigFileInfoHref = (webConfigResponse.SelectToken("$.file_info._links.self.href") ?? "").Value<string>();
                    if (string.IsNullOrWhiteSpace(webConfigFileInfoHref))
                    {
                        continue;
                    }

                    JObject webConfigFileInfoResponse = ExecuteJson(CreateRequest(webConfigFileInfoHref));
                    if (webConfigFileInfoResponse == null)
                    {
                        continue;
                    }

                    string contentHref = (webConfigFileInfoResponse.SelectToken("$._links.content.href") ?? "").Value<string>();
                    if (string.IsNullOrWhiteSpace(contentHref))
                    {
                        continue;
                    }

                    string content = null;
                    IRestRequest contentRequest = CreateRequest(contentHref);
                    try
                    {
                        byte[] contentBytes = DownloadData(contentRequest);
                        if (contentBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(contentBytes))
                            using (StreamReader sr = new StreamReader(ms))
                            {
                                content = sr.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("IisAdministrationClient download 'web.config' error", e);
                    }
                    
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        continue;
                    }

                    XDocument config = XDocument.Parse(content);
                    XElement dataElement = config.XPathSelectElement("//configuration/dotnetnuke/data");
                    if (dataElement == null)
                    {
                        continue;
                    }

                    string defaultProvider = dataElement.Attributes("defaultProvider").FirstOrDefault()?.Value;
                    if (string.IsNullOrWhiteSpace(defaultProvider))
                    {
                        continue;
                    }

                    XElement defaultProviderElement = config.XPathSelectElements("//configuration/dotnetnuke/data/providers/add")
                                                            .FirstOrDefault(e => e.HasAttributes && e.Attribute("name")?.Value == defaultProvider);

                    if (defaultProviderElement == null)
                    {
                        continue;
                    }

                    string connStrName = defaultProviderElement.Attribute("connectionStringName")?.Value;
                    siteInfo.ObjectQualifier = defaultProviderElement.Attribute("objectQualifier")?.Value;
                    siteInfo.DatabaseOwner = defaultProviderElement.Attribute("databaseOwner")?.Value;

                    if (string.IsNullOrWhiteSpace(connStrName))
                    {
                        continue;
                    }

                    XElement connStrElement = config.XPathSelectElements("//configuration/connectionStrings/add")
                                                    .FirstOrDefault(e => e.HasAttributes && e.Attribute("name")?.Value == connStrName);
                    if (connStrElement != null)
                    {
                        siteInfo.ConnectionString = connStrElement.Attribute("connectionString")?.Value;
                    }
                }

                siteInfos.Add(siteInfo);
            }

            return siteInfos;
        }
    }
}