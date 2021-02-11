#region Usings

using System;

#endregion

namespace Italliance.Modules.DnnHosting.Components.Mvc.Excel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelValueFormatAttribute : Attribute
    {
        public ExcelValueFormatAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; set; }
    }
}
