#region

using DotNetNuke.DependencyInjection;
using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Italliance.Modules.DnnHosting
{
    // ReSharper disable once UnusedType.Global
    public class Startup : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IMapper, Mapper>();
            services.AddTransient<IClientDataService, ClientDataService>();
        }
    }
}