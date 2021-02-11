#region Usings

using System.Web.Mvc;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using Italliance.Modules.DnnHosting.Models;

#endregion

namespace Italliance.Modules.DnnHosting.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : BaseDnnController<DnnHostingSettings>
    {
        [HttpGet]
        public ActionResult Settings()
        {
            return View(ModuleSettings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(DnnHostingSettings settings)
        {
            SettingsRepository.SaveSettings(ActiveModule, settings);
            ModuleController.SynchronizeModule (ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }
    }
}
