#region

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Collections;
using Italliance.Modules.DnnHosting.Models;
using R7.Dnn.Extensions.Data;

#endregion

namespace Italliance.Modules.DnnHosting.Services
{
    public class ClientDataService : IClientDataService
    {
        private readonly Dal2DataProvider _dataProvider;

        public ClientDataService()
        {
            _dataProvider = new Dal2DataProvider();
        }

        public Client GetClient(int clientId)
        {
            return _dataProvider.Get<Client, int>(clientId);
        }

        public IEnumerable<Client> GetClients(int portalId)
        {
            return _dataProvider.GetObjects<Client>(portalId);
        }

        public IPagedList<Client> GetClientsPaged(int portalId, int pageIndex, int pageSize)
        {
            return _dataProvider.GetPage<Client, int>(portalId, pageIndex, pageSize);
        }

        public IPagedList<ClientDto> GetClientDtosPaged(int portalId, int pageIndex, int pageSize, Func<Client, ClientDto> mapFunc)
        {
            IPagedList<Client> clientsPaged = GetClientsPaged(portalId, pageIndex, pageSize);
            return clientsPaged != null && clientsPaged.Count > 0
                       ? new PagedList<ClientDto>(clientsPaged.Where(c => c != null).Select(mapFunc), clientsPaged.TotalCount, clientsPaged.PageIndex, clientsPaged.PageSize)
                       : new PagedList<ClientDto>(new List<ClientDto>(), 0, 0, 10);
        }

        public IPagedList<ClientDto> GetClientDtosPagedSearch(int portalId, string name, string email, string domain, Func<Client, ClientDto> mapFunc)
        {
            IPagedList<Client> clientsPaged = _dataProvider.GetPage<Client>("WHERE PortalId = @0 AND (Name = @1 OR Email = @2 OR Domain LIKE @3)",
                                                                            0,
                                                                            100,
                                                                            portalId,
                                                                            name.Trim(),
                                                                            email.Trim(),
                                                                            $"%{domain.Trim()}%");
            return clientsPaged != null && clientsPaged.Count > 0
                       ? new PagedList<ClientDto>(clientsPaged.Where(c => c != null).Select(mapFunc), clientsPaged.TotalCount, clientsPaged.PageIndex, clientsPaged.PageSize)
                       : new PagedList<ClientDto>(new List<ClientDto>(), 0, 0, 10);
        }

        public IEnumerable<Client> GetClientsByStatus(int portalId, ClientStatus status)
        {
            return _dataProvider.GetObjects<Client>("WHERE PortalId = @0 AND ClientStatus = @1", portalId, (int) status);
        }

        public IPagedList<Client> GetClientsByStatusPaged(int portalId, int pageIndex, int pageSize, ClientStatus status)
        {
            return _dataProvider.GetPage<Client>("WHERE PortalId = @0 AND ClientStatus = @1", pageIndex, pageSize, portalId, (int) status);
        }

        public void AddClient(Client client)
        {
            _dataProvider.Add(client);
        }

        public void UpdateClient(Client client)
        {
            _dataProvider.Update(client);
        }

        public void DeleteClient(int clientId)
        {
            _dataProvider.Delete<Client, int>(clientId);
        }

        public IEnumerable<Client> GetNormalClients(int portalId, int daysToExpire)
        {
            DateTime dtToExpire = DateTime.Now.AddDays(daysToExpire);
            return _dataProvider.GetObjects<Client>("WHERE PortalId = @0 AND HostingEndDate > @1", portalId, dtToExpire);
        }

        public IEnumerable<Client> GetExpiredClients(int portalId)
        {
            DateTime dtToExpire = DateTime.Now;
            return _dataProvider.GetObjects<Client>("WHERE PortalId = @0 AND (ClientStatus = @1 OR HostingEndDate < @2)", portalId, (int) ClientStatus.Disabled, dtToExpire);
        }

        public IEnumerable<Client> GetPreExpiredClients(int portalId, int daysToExpire)
        {
            DateTime dtToExpire = DateTime.Now.AddDays(daysToExpire);
            return _dataProvider.GetObjects<Client>("WHERE PortalId = @0 AND ClientStatus != @1 AND HostingEndDate < @2", portalId, (int) ClientStatus.Disabled, dtToExpire);
        }

        public IEnumerable<ClientDto> GetClientDtos(int portalId, Func<Client, ClientDto> mapFunc)
        {
            List<Client> clients = _dataProvider.GetObjects<Client>(portalId).ToList();
            return clients.Count > 0 ? clients.Where(c => c != null).Select(mapFunc) : new List<ClientDto>();
        }
    }
}