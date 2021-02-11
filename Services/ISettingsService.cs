using DotNetNuke.Entities.Modules.Settings;
using Italliance.Modules.DnnHosting.Models;

namespace Italliance.Modules.DnnHosting.Services
{
    public interface ISettingsService
    {
        SettingsRepository<DnnHostingSettings> SettingsRepository { get; }
    }
}