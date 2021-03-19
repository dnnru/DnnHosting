#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Scheduling;
using HandlebarsDotNet;
using Italliance.Modules.DnnHosting.Models;
using Italliance.Modules.DnnHosting.Services;
using Italliance.Modules.DnnHosting.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Tasks
{
    public class DnnHostingScheduler : SchedulerClient
    {
        private readonly IClientDataService _dataService;
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public DnnHostingScheduler(ScheduleHistoryItem scheduleHistoryItem, ISettingsService settingsService, IClientDataService dataService, IMapper mapper)
        {
            _settingsService = settingsService;
            _dataService = dataService;
            _mapper = mapper;
            ScheduleHistoryItem = scheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                Progressing();
                bool success = Process(out string error);
                ScheduleHistoryItem.Succeeded = success;
                ScheduleHistoryItem.AddLogNote(error);
            }
            catch (Exception e)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote($"DnnHostingScheduler Error: {e.Message}");
                Errored(ref e);
                Exceptions.LogException(e);
            }
        }

        private bool Process(out string error)
        {
            bool result = false;
            error = "";
            StringBuilder errorBuilder = new StringBuilder();

            try
            {
                ModuleInfo moduleInfo = Utils.GetDnnHostingModules().FirstOrDefault();
                if (moduleInfo != null)
                {
                    DnnHostingSettings settings = _settingsService.SettingsRepository.GetSettings(moduleInfo);
                    if (settings != null)
                    {
                        string updateStatusError = UpdateStatus(settings, moduleInfo.PortalID);
                        if (!string.IsNullOrWhiteSpace(updateStatusError))
                        {
                            errorBuilder.AppendFormat("Update Status Error: {0}{1}", updateStatusError, Environment.NewLine);
                        }

                        string sendEmailToClientError = SendEmailToClient(settings, moduleInfo.PortalID);
                        if (!string.IsNullOrWhiteSpace(sendEmailToClientError))
                        {
                            errorBuilder.AppendFormat("Send Email To Client Error: {0}{1}", sendEmailToClientError, Environment.NewLine);
                        }

                        string sendEmailToAdminError = SendEmailToAdmin(settings, moduleInfo.PortalID);
                        if (!string.IsNullOrWhiteSpace(sendEmailToClientError))
                        {
                            errorBuilder.AppendFormat("Send Email To Admin Error: {0}{1}", sendEmailToAdminError, Environment.NewLine);
                        }
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                errorBuilder.AppendFormat("General Exception: {0}", e.Message);
            }

            error = errorBuilder.ToString();
            return result;
        }

        private string UpdateStatus(DnnHostingSettings settings, int portalId)
        {
            string result = "";
            try
            {
                foreach (Client client in _dataService.GetNormalClients(portalId, settings.DaysToExpire))
                {
                    client.ClientStatus = (int) ClientStatus.Ok;
                    _dataService.UpdateClient(client);
                }

                foreach (Client client in _dataService.GetPreExpiredClients(portalId, settings.DaysToExpire))
                {
                    client.ClientStatus = (int) ClientStatus.PaymentPending;
                    _dataService.UpdateClient(client);
                }

                foreach (Client client in _dataService.GetExpiredClients(portalId))
                {
                    client.ClientStatus = (int) ClientStatus.Disabled;
                    _dataService.UpdateClient(client);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }

        private string SendEmailToClient(DnnHostingSettings settings, int portalId)
        {
            string result = "";
            try
            {
                List<ClientDto> clients = new List<ClientDto>();
                clients.AddRange(_dataService.GetExpiredClients(portalId).Select(_mapper.MapClientDto));
                clients.AddRange(_dataService.GetPreExpiredClients(portalId, settings.DaysToExpire).Select(_mapper.MapClientDto));

                foreach (ClientDto client in clients)
                {
                    string source = settings.ClientEmailTemplate;
                    var template = Handlebars.Compile(source);
                    var compiledResult = template(client);
                    Mail.SendEmail(settings.EmailFrom, client.Email, string.Format(settings.ClientEmailSubject, client.Domain), compiledResult);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }

        private string SendEmailToAdmin(DnnHostingSettings settings, int portalId)
        {
            string result = "";
            try
            {
                AdminEmailViewModel model = new AdminEmailViewModel();
                model.ExpiredClients.AddRange(_dataService.GetExpiredClients(portalId).Select(_mapper.MapClientDto));
                model.PreExpiredClients.AddRange(_dataService.GetPreExpiredClients(portalId, settings.DaysToExpire).Select(_mapper.MapClientDto));
                if (model.HasExpiredClients || model.HasPreExpiredClients)
                {
                    string source = settings.AdminEmailTemplate;
                    string partialSource = settings.ClientPartialEmailTemplate;
                    Handlebars.RegisterTemplate("ClientPartial", partialSource);
                    var template = Handlebars.Compile(source);
                    var compiledResult = template(model);
                    Mail.SendEmail(settings.EmailFrom, settings.AdminEmail, settings.AdminEmailSubject, compiledResult);
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return result;
        }
    }
}