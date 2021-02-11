#region

using DotNetNuke.Services.Scheduling;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Tasks
{
    public static class SchedulerHelper
    {
        public static void CreateOrUpdateSchedule(ScheduleItem item = null)
        {
            bool create = false;
            if (item == null)
            {
                item = new ScheduleItem();
                create = true;
            }

            item.TypeFullName = Constants.SCHEDULER_TYPE_FULL_NAME;
            item.FriendlyName = "DnnHosting Scheduler";
            item.ScheduleSource = ScheduleSource.STARTED_FROM_TIMER;
            item.TimeLapse = 1;
            item.TimeLapseMeasurement = "d";
            item.RetryTimeLapse = 10;
            item.RetryTimeLapseMeasurement = "m";
            item.RetainHistoryNum = 10;
            item.CatchUpEnabled = false;
            item.Enabled = true;
            item.ObjectDependencies = "";

            if (create)
            {
                SchedulingProvider.Instance().AddSchedule(item);
            }
            else
            {
                SchedulingProvider.Instance().UpdateSchedule(item);
            }
        }
    }
}