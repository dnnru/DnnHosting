namespace Italliance.Modules.DnnHosting.Models
{
    public class EmailTemplates : IEmailTemplates
    {
        public string Admin { get; set; }
        public string Client { get; set; }
        public string ClientPartial { get; set; }
    }
}