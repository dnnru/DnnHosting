using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Models;
using R7.Dnn.Extensions.ViewModels;

namespace Italliance.Modules.DnnHosting.ViewModels
{
    public class ImportViewModel
    {
        public ImportViewModel(ViewModelContext<DnnHostingSettings> context)
        {
            Context = context;
        }
        
        public ViewModelContext<DnnHostingSettings> Context { get; }

        public string ModulePath => Constants.MODULE_PATH;
    }
}