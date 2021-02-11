namespace Italliance.Modules.DnnHosting.Components.Mvc.Bootstrap
{
    internal class BootstrapControl
    {
        public string Id { get; set; }

        public string ErrorMessage { get; set; }

        public string Class { get; set; }

        public bool IsValid => !Class.Contains("input-validation-error");
    }
}
