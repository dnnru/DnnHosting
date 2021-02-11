#region Usings

using Italliance.Modules.DnnHosting.Models;

#endregion

namespace Italliance.Modules.DnnHosting.Components
{
    public class Mapper : IMapper
    {
        public ClientDto MapClientDto(Client client)
        {
            return new ClientDto
                   {
                       ClientId = client.ClientId,
                       PortalId = client.PortalId,
                       CreatedByUserId = client.CreatedByUserId,
                       LastModifiedByUserId = client.LastModifiedByUserId,
                       CreatedOnDate = client.CreatedOnDate,
                       LastModifiedOnDate = client.LastModifiedOnDate,
                       Name = client.Name,
                       Email = client.Email,
                       Phone = client.Phone,
                       Domain = client.Domain,
                       HostingEndDate = client.HostingEndDate,
                       HostSpace = client.HostSpace,
                       PageQuota = client.PageQuota,
                       UserQuota = client.UserQuota,
                       PaymentPeriod = client.PaymentPeriod,
                       LastPaymentDate = client.LastPaymentDate,
                       PaymentMethod = client.PaymentMethod.ToEnum<PaymentMethod>(),
                       IsPaymentOk = client.IsPaymentOk,
                       ClientStatus = client.ClientStatus.ToEnum<ClientStatus>(),
                       Comments = client.Comments
                   };
        }

        public Client MapClient(ClientDto clientDto)
        {
            return new Client
                   {
                       PortalId = clientDto.PortalId,
                       CreatedByUserId = clientDto.CreatedByUserId,
                       LastModifiedByUserId = clientDto.LastModifiedByUserId,
                       CreatedOnDate = clientDto.CreatedOnDate,
                       LastModifiedOnDate = clientDto.LastModifiedOnDate,
                       ClientId = clientDto.ClientId,
                       Name = clientDto.Name,
                       Email = clientDto.Email,
                       Phone = clientDto.Phone,
                       Domain = clientDto.Domain,
                       HostingEndDate = clientDto.HostingEndDate,
                       HostSpace = clientDto.HostSpace,
                       PageQuota = clientDto.PageQuota,
                       UserQuota = clientDto.UserQuota,
                       PaymentPeriod = clientDto.PaymentPeriod,
                       LastPaymentDate = clientDto.LastPaymentDate,
                       PaymentMethod = (int) clientDto.PaymentMethod,
                       IsPaymentOk = clientDto.IsPaymentOk,
                       ClientStatus = (int) clientDto.ClientStatus,
                       Comments = clientDto.Comments
                   };
        }

        public void UpdateClient(ClientDto clientDto, Client client)
        {
            client.PortalId = clientDto.PortalId;
            client.CreatedByUserId = clientDto.CreatedByUserId;
            client.LastModifiedByUserId = clientDto.LastModifiedByUserId;
            client.CreatedOnDate = clientDto.CreatedOnDate;
            client.LastModifiedOnDate = clientDto.LastModifiedOnDate;
            client.ClientId = clientDto.ClientId;
            client.Name = clientDto.Name;
            client.Email = clientDto.Email;
            client.Phone = clientDto.Phone;
            client.Domain = clientDto.Domain;
            client.HostingEndDate = clientDto.HostingEndDate;
            client.HostSpace = clientDto.HostSpace;
            client.PageQuota = clientDto.PageQuota;
            client.UserQuota = clientDto.UserQuota;
            client.PaymentPeriod = clientDto.PaymentPeriod;
            client.LastPaymentDate = clientDto.LastPaymentDate;
            client.PaymentMethod = (int) clientDto.PaymentMethod;
            client.IsPaymentOk = clientDto.IsPaymentOk;
            client.ClientStatus = (int) clientDto.ClientStatus;
            client.Comments = clientDto.Comments;
        }
    }
}
