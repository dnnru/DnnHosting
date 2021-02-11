namespace Italliance.Modules.DnnHosting.Components.Mvc
{
    public class TemplatesConsts
    {
        public const string CURRENCY_FORMAT = "{0:C}";
        public const string DECIMAL_FORMAT = "{0:0.##}";
        private const string COL_LABEL = "4";
        private const string COL_CONTROL = "8";
        public const string LABEL = "col-sm-" + COL_LABEL + " control-label";
        private const string CONTROL_BASE = "col-sm-" + COL_CONTROL;
        public const string CONTROL = CONTROL_BASE + " control-value";
        public const string CONTROL_OFFSET = CONTROL + " col-sm-offset-" + COL_LABEL;
        public const string INPUT = "form-control";
        public const string CONTROL_RAW = CONTROL_BASE + " control-raw";
    }
}
