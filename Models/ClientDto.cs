#region

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using Italliance.Modules.DnnHosting.Components.Mvc.Excel;
using Italliance.Modules.DnnHosting.Components.Mvc.ModelBinders;

#endregion

namespace Italliance.Modules.DnnHosting.Models
{
    [ExcelSheetStyle(HeaderFontSize = 16, BodyFontSize = 14, FontFamily = "Times New Roman", IsHeaderBold = true)]
    [ModelBinder(typeof(CustomModelBinder))]
    public sealed class ClientDto
    {
        [Display(Name = "Client Id", Order = 0)]
        public int ClientId { get; set; }
        
        [Display(Name = "Client Name", Order = 5)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [Display(Name = "Client Email", Order = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Client Phone", Order = 20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Domains", Order = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
        public string Domain { get; set; }

        [Display(Name = "Hosting End Date", Order = 40)]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [ExcelValueFormat("dd.MM.yyyy")]
        [PropertyBinder(typeof(DateTimePropertyBinder))]
        public DateTime HostingEndDate { get; set; } = DateTime.Now;

        [Display(Name = "File system limit (MB)", Order = 50)]
        [Range(0, 10480, ErrorMessage = "Please use values between 0 to 10480")]
        public int HostSpace { get; set; } = 0;

        [Display(Name = "Pages limit", Order = 60)]
        [Range(0, 10000, ErrorMessage = "Please use values between 0 to 1000")]
        public int PageQuota { get; set; } = 0;

        [Display(Name = "Users limit", Order = 70)]
        [Range(0, 100000, ErrorMessage = "Please use values between 0 to 100000")]
        public int UserQuota { get; set; } = 0;

        [Display(Name = "Payment Period (months)", Order = 80)]
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[1-9][0-9]{0,2}$", ErrorMessage = "Invalid number value")]
        [Range(1, 99, ErrorMessage = "Please use values between 1 to 99")]
        public short PaymentPeriod { get; set; }

        [Display(Name = "Last Payment Date", Order = 90)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [ExcelValueFormat("dd.MM.yyyy")]
        [PropertyBinder(typeof(NullableDateTimePropertyBinder))]
        public DateTime? LastPaymentDate { get; set; }

        [Display(Name = "Payment Method", Order = 100)]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Payment Success", Order = 110)]
        public bool IsPaymentOk { get; set; }

        [Display(Name = "Status", Order = 120)]
        public ClientStatus ClientStatus { get; set; }

        [Display(Name = "Comments", Order = 130)]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Is Expired", Order = 140)]
        public bool IsExpired => ClientStatus == ClientStatus.Disabled || DaysToExpiry < 0;

        [Display(Name = "Days To Expiry", Order = 150)]
        public int DaysToExpiry => (HostingEndDate - DateTime.Now).Days;

        [Display(Name = "Created by User", Order = 160)]
        public string CreatedByUserName
        {
            get
            {
                if (CreatedByUserId == null)
                {
                    return null;
                }

                UserInfo user = ServiceLocator<IUserController, UserController>.Instance.GetUserById(PortalId, CreatedByUserId.Value);
                return user.Username;
            }
        }

        [Display(Name = "Last Modified by User", Order = 170)]
        public string LastModifiedByUserName
        {
            get
            {
                if (LastModifiedByUserId == null)
                {
                    return null;
                }

                UserInfo user = ServiceLocator<IUserController, UserController>.Instance.GetUserById(PortalId, LastModifiedByUserId.Value);
                return user.Username;
            }
        }

        [Display(Name = "Created On Date", Order = 180)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = false)]
        [ExcelValueFormat("dd.MM.yyyy HH:mm:ss")]
        [PropertyBinder(typeof(NullableDateTimePropertyBinder))]
        public DateTime? CreatedOnDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Modified On Date", Order = 190)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = false)]
        [ExcelValueFormat("dd.MM.yyyy HH:mm:ss")]
        [PropertyBinder(typeof(NullableDateTimePropertyBinder))]
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;

        [Display(Name = "PortalId")]
        [ExcelIgnore]
        public int PortalId { get; set; }
        
        [Display(Name = "Created By UserId")]
        [ExcelIgnore]
        public int? CreatedByUserId { get; set; }

        [Display(Name = "Last Modified By UserId")]
        [ExcelIgnore]
        public int? LastModifiedByUserId { get; set; }

        [ExcelIgnore]
        public bool SetupRequired { get; set; }
        
        [Display(Name = "Create/Edit Error")]
        [ExcelIgnore]
        public string ErrorMessage { get; set; }
    }
}