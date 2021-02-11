#region

using System;
using LinqToDB.Mapping;

#endregion

// ReSharper disable InconsistentNaming

namespace Italliance.Modules.DnnHosting.Models
{
    [Table("Portals")]
    public partial class Portal
    {
        [PrimaryKey]
        [Identity]
        public int PortalID { get; set; } // int

        [Column]
        [Nullable]
        public DateTime? ExpiryDate { get; set; } // datetime

        [Column]
        [NotNull]
        public int HostSpace { get; set; } // int

        [Column]
        [NotNull]
        public int PageQuota { get; set; } // int

        [Column]
        [NotNull]
        public int UserQuota { get; set; } // int
    }
}