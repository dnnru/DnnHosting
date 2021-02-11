#region Usings

using System;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    [TableName("DnnHosting_Clients")]
    [PrimaryKey("ClientId", AutoIncrement = true)]
    [Cacheable("DnnHosting_Clients", CacheItemPriority.Default, 20)]
    [Scope("PortalId")]
    public class Client : IClient
    {
        [ColumnName("ClientId")]
        public int ClientId { get; set; }

        [ColumnName("Name")]
        public string Name { get; set; }

        [ColumnName("Email")]
        public string Email { get; set; }

        [ColumnName("Phone")]
        public string Phone { get; set; }

        [ColumnName("Domain")]
        public string Domain { get; set; }

        [ColumnName("HostingEndDate")]
        public DateTime HostingEndDate { get; set; }
        
        [ColumnName("HostSpace")]
        public int HostSpace { get; set; }
        
        [ColumnName("PageQuota")]
        public int PageQuota { get; set; }
        
        [ColumnName("UserQuota")]
        public int UserQuota { get; set; }

        [ColumnName("PaymentPeriod")]
        public short PaymentPeriod { get; set; }

        [ColumnName("LastPaymentDate")]
        public DateTime? LastPaymentDate { get; set; }

        [ColumnName("PaymentMethod")]
        public int PaymentMethod { get; set; }

        [ColumnName("IsPaymentOk")]
        public bool IsPaymentOk { get; set; }

        [ColumnName("ClientStatus")]
        public int ClientStatus { get; set; }

        [ColumnName("Comments")]
        public string Comments { get; set; }

        [ColumnName("PortalId")]
        public int PortalId { get; set; }

        [ColumnName("CreatedByUserId")]
        public int? CreatedByUserId { get; set; }

        [ColumnName("LastModifiedByUserId")]
        public int? LastModifiedByUserId { get; set; }

        [ColumnName("CreatedOnDate")]
        public DateTime? CreatedOnDate { get; set; }

        [ColumnName("LastModifiedOnDate")]
        public DateTime? LastModifiedOnDate { get; set; }
    }
}
