#region Usings

using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Models;
using R7.Dnn.Extensions.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.ViewModels
{
    public class ClientViewModel
    {
        public ClientViewModel(ClientDto client, ViewModelContext<DnnHostingSettings> context)
        {
            Client = client;
            Context = context;
        }

        public ClientDto Client { get; }
        public ViewModelContext<DnnHostingSettings> Context { get; }
        public string ModulePath => Constants.MODULE_PATH;
    }
}
