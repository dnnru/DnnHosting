#region Usings

using Italliance.Modules.DnnHosting.Models;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public interface IMapper
    {
        ClientDto MapClientDto(Client client);
        Client MapClient(ClientDto clientDto);
        void UpdateClient(ClientDto clientDto, Client client);
    }
}
