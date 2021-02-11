namespace Italliance.Modules.DnnHosting.Models
{
    public interface IEmailTemplates
    {
        string Admin { get; set; }
        string Client { get; set; }
        string ClientPartial { get; set; }
    }
}