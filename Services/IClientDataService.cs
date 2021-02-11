#region Usings

using System;
using System.Collections.Generic;
using DotNetNuke.Collections;
using Italliance.Modules.DnnHosting.Models;

#endregion

namespace Italliance.Modules.DnnHosting.Services
{
    public interface IClientDataService
    {
        void AddClient(Client client);
        void DeleteClient(int clientId);
        Client GetClient(int clientId);
        IEnumerable<Client> GetClients(int portalId);
        IEnumerable<ClientDto> GetClientDtos(int portalId, Func<Client, ClientDto> mapFunc);
        IEnumerable<Client> GetClientsByStatus(int portalId, ClientStatus status);
        IPagedList<Client> GetClientsByStatusPaged(int portalId, int pageIndex, int pageSize, ClientStatus status);
        IPagedList<Client> GetClientsPaged(int portalId, int pageIndex, int pageSize);
        IPagedList<ClientDto> GetClientDtosPagedSearch(int portalId, string name, string email, string domain, Func<Client, ClientDto> mapFunc);
        IPagedList<ClientDto> GetClientDtosPaged(int portalId, int pageIndex, int pageSize, Func<Client, ClientDto> mapFunc);
        IEnumerable<Client> GetExpiredClients(int portalId);
        IEnumerable<Client> GetPreExpiredClients(int portalId, int daysToExpire);
        IEnumerable<Client> GetNormalClients(int portalId, int daysToExpire);
        void UpdateClient(Client client);
    }
}
