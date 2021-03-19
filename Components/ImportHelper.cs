#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DotNetNuke.Instrumentation;
using Italliance.Modules.DnnHosting.Components.ExcelDataExtractor;
using Italliance.Modules.DnnHosting.Models;
using Italliance.Modules.DnnHosting.Services;
using OfficeOpenXml;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public class ImportHelper
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ImportHelper));
        private readonly string _excelFile;

        public ImportHelper(string excelFile)
        {
            _excelFile = excelFile;
        }

        public bool Import(int portalId, int userId, IClientDataService dataService, string iisAdministrationBaseUrl, string iisAdministrationAccessToken, out string error)
        {
            error = "";
            bool result = true;
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(_excelFile)))
                {
                    int index = package.Compatibility.IsWorksheets1Based ? 1 : 0;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[index];
                    ExcelAddress excelAddress = worksheet.GetDataBounds();
                    if (excelAddress.IsEmptyRange())
                    {
                        error = "Empty import file";
                        return false;
                    }

                    IisAdministrationClient administrationClient = new IisAdministrationClient(iisAdministrationBaseUrl, iisAdministrationAccessToken);
                    List<SiteInfo> siteInfos = administrationClient.GetSiteInfosCached().ToList();

                    List<ClientDto> clientDtos = worksheet.Extract<ClientDto>()
                                                       .WithProperty(c => c.ClientId, "A", ToInt)
                                                       .WithProperty(c => c.Name, "B")
                                                       .WithProperty(c => c.Email, "C")
                                                       .WithProperty(c => c.Phone, "D")
                                                       .WithProperty(c => c.Domain, "E")
                                                       .WithProperty(c => c.HostingEndDate, "F", ToDateTime)
                                                       .WithProperty(c => c.HostSpace, "G", ToInt)
                                                       .WithProperty(c => c.PageQuota, "H", ToInt)
                                                       .WithProperty(c => c.UserQuota, "I", ToInt)
                                                       .WithProperty(c => c.PaymentPeriod, "J", ToShort)
                                                       .WithProperty(c => c.LastPaymentDate, "K", ToNullableDateTime)
                                                       .WithProperty(c => c.PaymentMethod, "L", ToPaymentMethod)
                                                       .WithProperty(c => c.IsPaymentOk, "M", ToBoolean)
                                                       .WithProperty(c => c.ClientStatus, "N", ToClientStatus)
                                                       .WithProperty(c => c.Comments, "O")
                                                       .GetData(excelAddress.Start.Row, excelAddress.End.Row)
                                                       .ToList();
                    
                    foreach (ClientDto importedClientDto in clientDtos)
                    {
                        Client client = dataService.GetClient(importedClientDto.ClientId) ?? new Client();
                        bool isNew = client.ClientId < 1;
                        client.Name = importedClientDto.Name;
                        client.Email = importedClientDto.Email;
                        client.Phone = importedClientDto.Phone;
                        client.Domain = importedClientDto.Domain;
                        client.HostingEndDate = importedClientDto.HostingEndDate;
                        client.HostSpace = importedClientDto.HostSpace;
                        client.PageQuota = importedClientDto.PageQuota;
                        client.UserQuota = importedClientDto.UserQuota;
                        client.PaymentPeriod = importedClientDto.PaymentPeriod;
                        client.LastPaymentDate = importedClientDto.LastPaymentDate;
                        client.PaymentMethod = (int) importedClientDto.PaymentMethod;
                        client.IsPaymentOk = importedClientDto.IsPaymentOk;
                        client.ClientStatus = (int) importedClientDto.ClientStatus;
                        client.Comments = importedClientDto.Comments;
                        client.LastModifiedByUserId = userId;
                        client.LastModifiedOnDate = DateTime.Now;

                        if (isNew)
                        {
                            client.PortalId = portalId;
                            client.CreatedByUserId = userId;
                            client.CreatedOnDate = DateTime.Now;
                            try
                            {
                                dataService.AddClient(client);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                                result = false;
                                error = $"{error} AddClient '{client.Name}' Error: {e.Message}{Environment.NewLine}";
                            }
                        }
                        else
                        {
                            try
                            {
                                dataService.UpdateClient(client);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                                result = false;
                                error = $"{error} UpdateClient '{client.Name}' Error: {e.Message}{Environment.NewLine}";
                            }
                        }

                        if (Utils.UpdatePortals(siteInfos, client, out string updatePortalsError))
                        {
                            continue;
                        }

                        result = false;
                        error = $"{error} UpdatePortals for client '{client.Name}' Error: {updatePortalsError}{Environment.NewLine}";
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                result = false;
                error = $"{error} Global Error: {e.Message}{Environment.NewLine}";
            }

            return result;
        }

        internal ClientStatus ToClientStatus(object value)
        {
            if (value == null)
            {
                return ClientStatus.Unknown;
            }

            return Enum.TryParse(value.ToString(), true, out ClientStatus result) ? result : ClientStatus.Unknown;
        }

        internal bool ToBoolean(object value)
        {
            if (value == null)
            {
                return false;
            }

            switch (value.ToString().ToLower())
            {
                case "true":
                case "1":
                case "истина":
                    return true;
                case "false":
                case "0":
                case "ложь":
                    return false;
            }

            return false;
        }

        internal DateTime? ToNullableDateTime(object value)
        {
            return value.ToDateTime();
        }

        internal PaymentMethod ToPaymentMethod(object value)
        {
            if (value == null)
            {
                return PaymentMethod.Other;
            }

            return Enum.TryParse(value.ToString(), true, out PaymentMethod result) ? result : PaymentMethod.Other;
        }

        internal DateTime ToDateTime(object value)
        {
            DateTime? result = value.ToDateTime();
            return result ?? DateTime.MinValue;
        }

        internal short ToShort(object value)
        {
            if (value == null)
            {
                return 0;
            }

            return short.TryParse(value.ToString(), out short result) ? result : (short)0;
        }

        internal int ToInt(object value)
        {
            if (value == null)
            {
                return 0;
            }

            return int.TryParse(value.ToString(), out int result) ? result : 0;
        }
    }
}