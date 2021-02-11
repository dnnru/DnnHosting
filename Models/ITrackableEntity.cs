using System;

namespace Italliance.Modules.DnnHosting.Models
{
    public interface ITrackableEntity
    {
        int? CreatedByUserId { get; set; }

        int? LastModifiedByUserId { get; set; }

        DateTime? CreatedOnDate { get; set; }

        DateTime? LastModifiedOnDate { get; set; }
    }
}
