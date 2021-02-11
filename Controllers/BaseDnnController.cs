#region Usings

using System.Collections.Generic;
using DotNetNuke.Entities.Modules.Settings;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Italliance.Modules.DnnHosting.Components;
using Italliance.Modules.DnnHosting.Components.Mvc.Excel;
using R7.Dnn.Extensions.Models;
using R7.Dnn.Extensions.ViewModels;

#endregion

namespace Italliance.Modules.DnnHosting.Controllers
{
    public abstract class BaseDnnController<TSettings> : DnnController where TSettings : class, new()
    {
        private TSettings _settings;

        private SettingsRepository<TSettings> _settingsRepo;

        /// <summary>
        ///     Gets strongly-typed module settings.
        /// </summary>
        /// <value>The module settings.</value>
        protected TSettings ModuleSettings => _settings ?? (_settings = SettingsRepository.GetSettings(ModuleContext.Configuration));

        protected string ModulePath => Constants.MODULE_PATH;

        protected ViewModelContext<TSettings> Context => new ViewModelContext<TSettings>(ModuleContext, LocalResourceFile, ModuleSettings);

        /// <summary>
        ///     Gets or sets the settings repository.
        /// </summary>
        /// <value>The settings repository.</value>
        protected SettingsRepository<TSettings> SettingsRepository
        {
            get => _settingsRepo ?? (_settingsRepo = CreateSettingsRepository());
            set
            {
                _settingsRepo = value;
                _settings = SettingsRepository.GetSettings(ModuleContext.Configuration);
            }
        }

        /// <summary>
        ///     Creates the settings repository.
        /// </summary>
        /// <returns>The settings repository.</returns>
        protected virtual SettingsRepository<TSettings> CreateSettingsRepository()
        {
            return new SettingsRepositoryImpl<TSettings>();
        }

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data) where T : class
        {
            return new ExcelResult<T>(data);
        }

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data, string fileName) where T : class
        {
            return new ExcelResult<T>(data, fileName);
        }

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data, string fileName, string sheetName) where T : class
        {
            return new ExcelResult<T>(data, fileName, sheetName);
        }
    }
}
