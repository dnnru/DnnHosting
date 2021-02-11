#region Usings

using DotNetNuke.Collections;
using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Models;
using R7.Dnn.Extensions.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.ViewModels
{
    public class ClientListViewModel
    {
        public ClientListViewModel(IPagedList<ClientDto> clients, ViewModelContext<DnnHostingSettings> context)
        {
            Clients = clients;
            Context = context;
        }

        public IPagedList<ClientDto> Clients { get; }
        public ViewModelContext<DnnHostingSettings> Context { get; }

        public string ModulePath => Constants.MODULE_PATH;
    }
}
