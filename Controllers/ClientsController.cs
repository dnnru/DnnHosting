#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Models;
using Italliance.Modules.DnnHosting.Services;
using Italliance.Modules.DnnHosting.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.Controllers
{
    [DnnHandleError]
    public class ClientsController : BaseDnnController<DnnHostingSettings>
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ClientsController));
        private readonly IClientDataService _dataService;
        private readonly IMapper _mapper;

        public ClientsController(IClientDataService dataService, IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        public ActionResult Delete(int clientId)
        {
            try
            {
                _dataService.DeleteClient(clientId);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return RedirectToDefaultRoute();
        }

        [DnnAuthorize]
        public ActionResult Import()
        {
            return View(new ImportViewModel(Context));
        }

        [DnnAuthorize]
        public JsonResult Upload()
        {
            try
            {
                string path = Server.MapPath("~/Portals/_default/DnnHosting/Import");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (Request.Files.Count <= 0)
                {
                    return Json(new {success = false, message = "No uploaded files!"}, JsonRequestBehavior.AllowGet);
                }

                var file = Request.Files[0];
                if (file == null || file.ContentLength <= 0)
                {
                    return Json(new {success = false, message = "Invalid uploaded file!"}, JsonRequestBehavior.AllowGet);
                }

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(path, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                file.SaveAs(filePath);
                ImportHelper excelHelper = new ImportHelper(filePath);
                bool isImportSuccess = excelHelper.Import(PortalSettings.PortalId,
                                                          User.UserID,
                                                          _dataService,
                                                          ModuleSettings.IisAdministrationUrl,
                                                          ModuleSettings.IisAdministrationApiKey,
                                                          out string error);
                return Json(!isImportSuccess ? new {success = false, message = error} : new {success = true, message = "Import Success!"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new {success = false, message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int clientId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            Client client = _dataService.GetClient(clientId);
            ClientDto clientDto = client == null ? new ClientDto() : _mapper.MapClientDto(client);
            clientDto.SetupRequired = string.IsNullOrWhiteSpace(ModuleSettings.IisAdministrationApiKey);
            return View(clientDto);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(ClientDto clientDto)
        {
            if (ModelState.IsValid)
            {
                Client client;
                clientDto.LastModifiedByUserId = User.UserID;
                clientDto.LastModifiedOnDate = DateTime.UtcNow;
                clientDto.PortalId = PortalSettings.PortalId;
                clientDto.ErrorMessage = "";

                try
                {
                    if (clientDto.ClientId < 1)
                    {
                        clientDto.CreatedByUserId = User.UserID;
                        clientDto.CreatedOnDate = DateTime.UtcNow;
                        client = _mapper.MapClient(clientDto);
                        _dataService.AddClient(client);
                    }
                    else
                    {
                        client = _dataService.GetClient(clientDto.ClientId);
                        if (client != null)
                        {
                            _mapper.UpdateClient(clientDto, client);
                            _dataService.UpdateClient(client);
                        }
                    }

                    IisAdministrationClient iisAdministrationClient = new IisAdministrationClient(ModuleSettings.IisAdministrationUrl, ModuleSettings.IisAdministrationApiKey);
                    List<SiteInfo> siteInfos = iisAdministrationClient.GetSiteInfosCached().ToList();
                    if (!Utils.UpdatePortals(siteInfos, client, out string error))
                    {
                        clientDto.ErrorMessage = error;
                        Logger.Error(error);
                        return View(clientDto);
                    }
                }
                catch (Exception e)
                {
                    clientDto.ErrorMessage = e.Message;
                    Exceptions.LogException(e);
                    Logger.Error(e);
                    return View(clientDto);
                }

                return RedirectToDefaultRoute();
            }

            return View(clientDto);
        }

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddClient")]
        public ActionResult Index(int page = 0, string name = "", string email = "", string domain = "")
        {
            IPagedList<ClientDto> clientDtos;
            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(domain))
            {
                clientDtos = _dataService.GetClientDtosPagedSearch(PortalSettings.PortalId,
                                                                   HttpUtility.UrlDecode(name),
                                                                   HttpUtility.UrlDecode(email),
                                                                   HttpUtility.UrlDecode(domain),
                                                                   _mapper.MapClientDto);
            }
            else
            {
                int pageIndex = page > 0 ? page - 1 : 0;
                clientDtos = _dataService.GetClientDtosPaged(PortalSettings.PortalId, pageIndex, ModuleSettings.PageSize, _mapper.MapClientDto);
            }

            ClientListViewModel model = new ClientListViewModel(clientDtos, Context);
            return View(model);
        }

        public ActionResult ExportToExcel()
        {
            List<ClientDto> clients = _dataService.GetClientDtos(PortalSettings.PortalId, _mapper.MapClientDto).ToList();
            return Excel(clients, $"clients_{DateTime.Now:yyyyMMddHHmmss}", "clients");
        }
    }
}