#region Usings

using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Scheduling;
using Italliance.Modules.DnnHosting.Components.Tasks; //using System.Xml;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The Controller class for DnnHosting
    ///     The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    ///     DotNetNuke will poll this class to find out which Interfaces the class implements.
    ///     The IPortable interface is used to import/export content from a DNN module
    ///     The ISearchable interface is used by DNN to index the content of a module
    ///     The IUpgradeable interface allows module developers to execute code during the upgrade
    ///     process for a module.
    ///     Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------

    //uncomment the interfaces to add the support.
    public class FeatureController : IUpgradeable //IPortable, ISearchable, 
    {
        public string UpgradeModule(string version)
        {
            try
            {
                ScheduleItem task = SchedulingController.GetSchedule().FirstOrDefault(x => x.TypeFullName == Constants.SCHEDULER_TYPE_FULL_NAME);
                SchedulerHelper.CreateOrUpdateSchedule(task);

                return "Success";
            }
            catch (Exception ex)
            {
                return "Failed: " + ex.Message;
            }
        }
    }
}
