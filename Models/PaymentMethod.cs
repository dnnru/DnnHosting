#region Usings

using System.ComponentModel.DataAnnotations;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    public enum PaymentMethod : int
    {
        [Display(Name = "Cash")]
        Cash = 0,

        [Display(Name = "Cashless")]
        Cashless = 1,

        [Display(Name = "Other")]
        Other = 2
    }
}
