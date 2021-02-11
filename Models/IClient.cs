#region Usings

using System;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    public interface IClient : ITrackableEntity
    {
        int PortalId { get; set; }

        int ClientId { get; set; }

        string Name { get; set; }

        string Email { get; set; }

        string Phone { get; set; }

        string Domain { get; set; }

        DateTime HostingEndDate { get; set; }
        
        int HostSpace { get; set; }

        int PageQuota { get; set; }

        int UserQuota { get; set; }

        short PaymentPeriod { get; set; }

        DateTime? LastPaymentDate { get; set; }

        int PaymentMethod { get; set; }

        bool IsPaymentOk { get; set; }

        int ClientStatus { get; set; }

        string Comments { get; set; }
    }
}
