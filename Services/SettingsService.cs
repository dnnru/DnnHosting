#region Usings

using DotNetNuke.Entities.Modules.Settings;
using Italliance.Modules.DnnHosting.Models;
using R7.Dnn.Extensions.Models;

#endregion

namespace Italliance.Modules.DnnHosting.Services
{
    public class SettingsService : ISettingsService
    {
        private SettingsRepository<DnnHostingSettings> _settings;
        public SettingsRepository<DnnHostingSettings> SettingsRepository => _settings ?? (_settings = CreateSettingsRepository());

        private SettingsRepository<DnnHostingSettings> CreateSettingsRepository()
        {
            return new SettingsRepositoryImpl<DnnHostingSettings>();
        }
    }
}
