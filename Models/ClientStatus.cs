#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    public enum ClientStatus : int
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        [Display(Name = "Ok")]
        Ok = 1,

        [Display(Name = "Payment Pending")]
        PaymentPending = 2,

        [Display(Name = "Disabled")]
        Disabled = 3
    }
}
